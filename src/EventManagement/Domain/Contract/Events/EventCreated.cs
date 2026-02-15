using Quantum.Domain;

namespace XEvent.EventManagement.Domain.Contract.Events;

public record EventCreated(
    long EventId,
    long Owner,
    string Name,
    DateTime StartDate,
    DateTime EndDate
) : IsADomainEvent;

public record EventTicketsIsSet(
    long EventId,
    ICollection<TicketDto> Tickets) : IsADomainEvent;


public record NewEventTicketIsAddedEvent(
    long EventId,
    TicketDto Ticket) : IsADomainEvent;

public record EventTicketIsUpdatedEvent(
    long EventId,
    long TicketId,
    TicketDto Ticket) : IsADomainEvent;

public record ATicketIsRemovedFromEvent(
    long EventId,long TicketId) : IsADomainEvent;

public record TicketDto(string Name, decimal Price, short Capacity);
public record EventUpdated(
    long EventId,   
    string Name,
    DateTime StartDate,
    DateTime EndDate
) : IsADomainEvent;