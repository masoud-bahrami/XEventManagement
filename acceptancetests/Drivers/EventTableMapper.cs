using System.Text;
using SpecFlow.Internal.Json;
using XEvent.EventManagement.Domain.Contract.Commands;
using XEvent.EventManagement.Domain.Contract.Events;

namespace XEvent.AcceptanceTests.Drivers;

public static class EventTableMapper
{
    public static CreateNewEventCommand ToCommand(this Table eventTable)
    {
        var name = eventTable.Rows[0]["Name"];
        var capacity =short.Parse(eventTable.Rows[0]["Capacity"]);
        var startDate =DateTime.Parse(eventTable.Rows[0]["StartDate"]);
        var endDate = DateTime.Parse(eventTable.Rows[0]["EndDate"]);

        return new CreateNewEventCommand(name, new List<TicketDto>()
        {
            new TicketDto("name", 20000, capacity)
        }, startDate, endDate);
    }
}