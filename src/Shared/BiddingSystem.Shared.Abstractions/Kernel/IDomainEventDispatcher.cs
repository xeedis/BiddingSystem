namespace BiddingSystem.Shared.Abstractions.Kernel;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(params IDomainEvent[] domainEvents);
}