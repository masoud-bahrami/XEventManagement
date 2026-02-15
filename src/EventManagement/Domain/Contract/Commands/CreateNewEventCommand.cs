using XEvent.EventManagement.Domain.Contract.Events;

namespace XEvent.EventManagement.Domain.Contract.Commands;

public  record  IsACommand{}
public record CreateNewEventCommand(string Name, 
    
    ICollection<TicketDto> Tickets,
    DateTime StartDate, DateTime EndDate) : IsACommand
{

}

public record DeleteTicketCommand(long TicketId) : IsACommand;
public record AddNewTicketCommand(TicketDto Ticket) : IsACommand;
public record UpdateTicketsCommand( TicketDto Ticket) : IsACommand;

public record UpdateEventCommand(long Id, string Name,  DateTime StartDate, DateTime EndDate) : IsACommand;