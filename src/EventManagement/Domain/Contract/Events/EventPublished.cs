using Quantum.Domain;

namespace XEvent.EventManagement.Domain.Contract.Events;

public record EventPublished(long EventId) : IsADomainEvent;