using Quantum.Domain;

namespace XEvent.Tickets.Domain.Contracts.Events;

public record ATicketIsRemovedFromEvent(
    long EventId,long TicketId) : IsADomainEvent;