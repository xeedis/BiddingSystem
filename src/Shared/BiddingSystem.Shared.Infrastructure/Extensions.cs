using System.Reflection;
using BiddingSystem.Shared.Abstractions.Modules;
using BiddingSystem.Shared.Infrastructure.Api;
using BiddingSystem.Shared.Infrastructure.Commands;
using BiddingSystem.Shared.Infrastructure.Modules;
using BiddingSystem.Shared.Infrastructure.Queries;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BiddingSystem.Shared.Infrastructure;

internal static class Extensions
{
    private const string CorsPolicy = "cors";

    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        IList<Assembly> assemblies, IList<IModule> modules)
    {
        var disabledModules = new List<string>();
        using (var serviceProvider = services.BuildServiceProvider())
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            foreach (var (key, value) in configuration.AsEnumerable())
            {
                if (!key.Contains(":module:enabled")) continue;

                if (!bool.Parse(value!))
                {
                    disabledModules.Add(key.Split(":")[0]);
                }
            }
        }
        
        services.AddCors(cors =>
        {
            cors.AddPolicy(CorsPolicy, x =>
            {
                x.WithOrigins("*")
                    .WithMethods("POST", "PUT", "DELETE")
                    .WithHeaders("Content-Type", "Authorization");
            });
        });
        
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddModuleInfo(modules);
        services.AddModuleRequests(assemblies);
        services.AddCommands(assemblies);
        services.AddQueries(assemblies);
        
        services.AddControllers()
            .ConfigureApplicationPartManager(manager =>
            {
                var removedParts = new List<ApplicationPart>();
                foreach (var disabledModule in disabledModules)
                {
                    var parts = manager.ApplicationParts.Where(x=>x.Name.Contains(disabledModule,
                        StringComparison.InvariantCultureIgnoreCase));
                    removedParts.AddRange(parts);
                }

                foreach (var part in removedParts)
                {
                    manager.ApplicationParts.Remove(part);
                }
                
                manager.FeatureProviders.Add(new InternalControllerFeatureProvider());
            });
        return services;
    }
    
    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
    {
        app.UseCors();
        
        app.UseAuthentication();
        app.UseRouting();
        app.UseAuthorization();

        return app;
    }
    
    public static T GetOptions<T>(this IServiceCollection services, string sectionName) where T : new()
    {
        using var serviceProvider = services.BuildServiceProvider();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        return configuration.GetOptions<T>(sectionName);
    }

    public static T GetOptions<T>(this IConfiguration configuration, string sectionName) where T : new()
    {
        var options = new T();
        configuration.GetSection(sectionName).Bind(options);
        return options;
    }
    
    public static string GetModuleName(this object value)
        => value?.GetType().GetModuleName() ?? string.Empty;

    public static string GetModuleName(this Type type)
    {
        if (type?.Namespace is null)
        {
            return string.Empty;
        }

        return type.Namespace.StartsWith("BiddingSystem.Modules.")
            ? type.Namespace.Split(".")[2].ToLowerInvariant()
            : string.Empty;
    }
}