using Quantum.Domain;
using XEvent.Tickets.Domain.Contracts.Commands;
using XEvent.Tickets.Domain.Contracts.Events;

namespace XEvent.Tickets.Domain.Contracts;

public static class CommandEventuation
{

    public static IEnumerable<IsADomainEvent> EventuateTo(
        this AddNewTicketCommand command, long eventId, long ticketId)
    {
        yield return new NewEventTicketIsAddedEvent(eventId, ticketId, command.Ticket);
    }
    public static IEnumerable<IsADomainEvent> EventuateTo(
        this DeleteTicketCommand command, long eventId)
    {
        yield return new ATicketIsRemovedFromEvent(eventId, command.TicketId);
    }
    
    public static IEnumerable<IsADomainEvent> EventuateTo(
        this UpdateTicketsCommand command,long eventId,  long ticketId)
    {
        yield return new EventTicketIsUpdatedEvent(eventId, ticketId,
            command.Ticket);
    }
}