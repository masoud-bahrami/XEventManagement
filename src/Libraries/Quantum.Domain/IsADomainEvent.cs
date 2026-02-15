namespace Quantum.Domain;

public abstract record IsADomainEvent
{
    public Guid AggregateId { get; init; }
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    public int Version { get; init; }
}