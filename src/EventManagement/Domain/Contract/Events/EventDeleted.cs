using Quantum.Domain;

public record EventDeleted(long EventId) : IsADomainEvent;