using Quantum.Domain;

public record EventMovedToDraft(long EventId) : IsADomainEvent;