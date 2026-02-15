using Microsoft.Extensions.DependencyInjection;
using Quantum.Domain;
using XEvent.EventManagement.Domain;
using XEvent.EventManagement.Domain.Contract;
using XEvent.EventManagement.Domain.Contract.Commands;
using XEvent.EventManagement.Domain.Contract.Events;
using XEvent.EventManagement.UseCaseHandlers;

namespace XEvent.AcceptanceTests.Drivers;

public class EventManagementTestFramework
{
    private readonly IEventRepository _repository;

    static readonly DateTime TODAY = DateTime.Now;
    static readonly DateTime YESTERDAY = DateTime.Now.AddDays(-1);
    const int OWNER = 1;

    public EventManagementTestFramework(IServiceProvider sp)
    {
        _repository = sp.CreateScope().ServiceProvider.GetService<IEventRepository>();
    }

    public async Task INeedToHaveTheseEvents(Table table)
    {
        int i = 1;
        foreach (var tableRow in table.Rows)
        {
            var events = ToEvents(tableRow, i++);

            var @event = new Event(events);
            await _repository.Save(@event);
        }
    }

    private static ICollection<IsADomainEvent> ToEvents(TableRow tableRow, int eventId)
    {
        var expectedName = tableRow["Name"];
        var expectedStatus = tableRow["Status"];
        
        var createNewEventCommand = new CreateNewEventCommand(expectedName, CreateASampleValidTickets(), YESTERDAY, TODAY);

        var events = new List<IsADomainEvent>(createNewEventCommand.EventuateTo(eventId, OWNER));

        
        if (Enum.TryParse(expectedStatus, out Status status))
        {
            switch (status)
            {
                case Status.Published:
                    events.Add(new EventPublished(eventId));
                    break;
                case Status.Cancelled:
                    events.Add(new EventCancelled(eventId));
                    break;
            }
        }

        return events;

        List<TicketDto> CreateASampleValidTickets()
        {
            return new List<TicketDto> {new("name", 20000, 10)};
        }
    }
}