using XEvent.EventManagement.Domain;
using XEvent.EventManagement.Domain.Contract.Events;

namespace XEvent.Reporting;

public record EventViewModel(long Id, long Owner,
    string Name, 
    ICollection<TicketDto> Tickets, DateTime StartDate, DateTime EndDate, Status Status);