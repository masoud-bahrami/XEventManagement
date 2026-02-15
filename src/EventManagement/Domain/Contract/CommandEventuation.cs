using Quantum.Domain;
using XEvent.EventManagement.Domain.Contract.Commands;
using XEvent.EventManagement.Domain.Contract.Events;

namespace XEvent.EventManagement.Domain.Contract;

public static class CommandEventuation
{
    public static IEnumerable<IsADomainEvent> EventuateTo(
        this CreateNewEventCommand command,
        long eventId,
        long owner)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
            throw new ArgumentException("Event name is required");
        
        if (command.EndDate <= command.StartDate)
            throw new ArgumentException("EndDate must be after StartDate");

        yield return new EventCreated(
            eventId,
            owner,
            command.Name,
            command.StartDate,
            command.EndDate
        );

        

        yield return new EventTicketsIsSet(eventId, command.Tickets);
    }

    public static IEnumerable<IsADomainEvent> EventuateTo(
        this UpdateEventCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
            throw new ArgumentException("Event name is required");
        
        if (command.EndDate <= command.StartDate)
            throw new ArgumentException("EndDate must be after StartDate");

        yield return new EventUpdated(
            command.Id,
            command.Name,
            command.StartDate,
            command.EndDate
        );
    }


    public static IEnumerable<IsADomainEvent> EventuateTo(
        this AddNewTicketCommand command, long eventId)
    {
        yield return new NewEventTicketIsAddedEvent(eventId, command.Ticket);
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