using Quantum.Domain;

namespace XEvent.Tickets.Domain.Contracts.Events;

public record EventTicketIsUpdatedEvent(
    long EventId,
    long TicketId,
    TicketDto Ticket) : IsADomainEvent;