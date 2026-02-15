using Quantum.Domain;

public record EventCancelled(long EventId) : IsADomainEvent;