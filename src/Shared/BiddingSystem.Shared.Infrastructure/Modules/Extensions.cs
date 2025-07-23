using System.Reflection;
using BiddingSystem.Shared.Abstractions.Commands;
using BiddingSystem.Shared.Abstractions.Events;
using BiddingSystem.Shared.Abstractions.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ICommand = System.Windows.Input.ICommand;

namespace BiddingSystem.Shared.Infrastructure.Modules;

public static class Extensions
{
    internal static IServiceCollection AddModuleInfo(this IServiceCollection services, IList<IModule> modules)
    {
        var moduleInfoProvider = new ModuleInfoProvider();
        var moduleInfo = modules?.Select(x=>new ModuleInfo(x.Name, x.Path, x.Policies ?? Enumerable.Empty<string>())) ??
                         Enumerable.Empty<ModuleInfo>();
        moduleInfoProvider.Modules.AddRange(moduleInfo);
        services.AddSingleton(moduleInfoProvider);
        
        return services;
    }

    internal static void MapModuleInfo(this IEndpointRouteBuilder endpoint)
    {
        endpoint.MapGet("modules", context =>
        {
            var moduleInfoProvider = context.RequestServices.GetRequiredService<ModuleInfoProvider>();
            return context.Response.WriteAsJsonAsync(moduleInfoProvider.Modules);
        });
    }
    
    public static IHostBuilder ConfigureModules(this IHostBuilder builder)
        => builder.ConfigureAppConfiguration((ctx, cfg) =>
        {
            foreach (var settings in GetSettings("*"))
            {
                cfg.AddJsonFile(settings);
            }

            foreach (var settings in GetSettings($"*.{ctx.HostingEnvironment.EnvironmentName}"))
            {
                cfg.AddJsonFile(settings);
            }
            
            IEnumerable<string> GetSettings(string pattern)
                => Directory.EnumerateFiles(
                    ctx.HostingEnvironment.ContentRootPath, $"module.{pattern}.json", SearchOption.AllDirectories);
        });

    internal static IServiceCollection AddModuleRequests(this IServiceCollection services,
        IList<Assembly> assemblies)
    {
        services.AddModuleRegistry(assemblies);
        services.AddSingleton<IModuleClient, ModuleClient>();
        services.AddSingleton<IModuleSerializer, JsonModuleSerializer>();
        services.AddSingleton<IModuleSubscriber, ModuleSubscriber>();

        return services;
    }

    public static IModuleSubscriber UseModuleRequest(this IApplicationBuilder app)
        => app.ApplicationServices.GetRequiredService<IModuleSubscriber>();

    private static void AddModuleRegistry(this IServiceCollection services, IEnumerable<Assembly> assemblies)
    {
        var registry = new ModuleRegistry();
        var types = assemblies.SelectMany(x => x.GetTypes()).ToArray();
            
        var commandTypes = types
            .Where(t => t.IsClass && typeof(ICommand).IsAssignableFrom(t))
            .ToArray();

        services.AddSingleton<IModuleRegistry>(sp =>
        {
            var commandDispatcher = sp.GetRequiredService<ICommandDispatcher>();
            var commandDispatcherType = commandDispatcher.GetType();
            var openSend = commandDispatcherType.GetMethod(nameof(commandDispatcher.SendAsync))
                ?? throw new MissingMethodException(commandDispatcherType.FullName,
                    nameof(ICommandDispatcher.SendAsync));
            
            foreach (var type in commandTypes)
            {
                var closedSend = openSend.MakeGenericMethod(type);
                registry.AddBroadcastAction(type, @event =>
                    (Task)(closedSend.Invoke(null,  [@event]) ?? throw new InvalidOperationException()));
            }

            return registry;
        });
    }
}