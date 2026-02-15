using Quantum.Domain;

namespace XEvent.Tickets.Domain.Contracts.Events;

public record NewEventTicketIsAddedEvent(
    long EventId,
    long TicketId,
    TicketDto Ticket) : IsADomainEvent;