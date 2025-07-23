using BiddingSystem.Shared.Abstractions.Time;

namespace BiddingSystem.Shared.Infrastructure.Time;

internal class UtcClock : IClock
{
    public DateTime CurrentDate() => DateTime.UtcNow;
}