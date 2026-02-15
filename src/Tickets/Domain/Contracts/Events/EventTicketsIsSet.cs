using Quantum.Domain;

namespace XEvent.Tickets.Domain.Contracts.Events;

public record EventTicketsIsSet(
    long EventId,
    ICollection<TicketDto> Tickets) : IsADomainEvent;