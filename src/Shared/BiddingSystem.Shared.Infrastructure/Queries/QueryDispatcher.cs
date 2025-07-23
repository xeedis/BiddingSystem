using BiddingSystem.Shared.Abstractions.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace BiddingSystem.Shared.Infrastructure.Queries;

internal sealed class QueryDispatcher : IQueryDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public QueryDispatcher(IServiceProvider serviceProvider)
        => _serviceProvider = serviceProvider;
    
    public async Task<TResult> QueryAsync<TResult>(IQuery<TResult> query)
    {
        using var scope = _serviceProvider.CreateScope();
        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult)) 
                          ?? throw new MissingMethodException();
        var handler = scope.ServiceProvider.GetRequiredService(handlerType);
        
        var handleMethod = handlerType.GetMethod(
                               nameof(IQueryHandler<IQuery<TResult>, TResult>.HandleAsync))
                           ?? throw new MissingMethodException(handlerType.FullName,
                               nameof(IQueryHandler<IQuery<TResult>, TResult>.HandleAsync));
        var returned = handleMethod
            .Invoke(handler, [ query ]) ?? throw new MissingMethodException();
        
        return returned is Task<TResult> task
            ? await task
            : throw new InvalidCastException(
                $"HandleAsync should return Task<{typeof(TResult).Name}>, but returned {returned.GetType().Name}.");
    }
}