using XEvent.EventManagement.Domain;
using XEvent.EventManagement.Domain.Contract.Commands;
using XEvent.EventManagement.Domain.Contract.Events;

internal static class EventTestFactory
{
    public static readonly long DefaultEventId = 1;
    public static readonly long DefaultOwnerId = 1;

    public static Event CreateDraftEvent(
        string name = "My Event",
        short capacity = 100,
        DateTime? start = null,
        DateTime? end = null)
    {
        var command = CreateCreateCommand(name, capacity, start, end);
        return new Event(DefaultEventId, DefaultOwnerId, command);
    }

    public static Event CreatePublishedEvent()
    {
        var ev = CreateDraftEvent();
        ev.Handle(CreatePublishCommand());
        ev.ClearUncommittedEvents();
        return ev;
    }
    
    public static Event CreateCanceledEvent()
    {
        var ev = CreateDraftEvent();
        ev.Handle(CreateCancelCommand());
        ev.ClearUncommittedEvents();
        return ev;
    }

    public static CreateNewEventCommand CreateCreateCommand(
        string name = "My Event",
        short capacity = 100,
        DateTime? start = null,
        DateTime? end = null)
    {
        return new CreateNewEventCommand(
            name,
            CreateSingleTicket(capacity),
            start ?? DateTime.UtcNow.AddDays(1),
            end ?? DateTime.UtcNow.AddDays(2)
        );
    }

    public static UpdateTicketsCommand CreateUpdateTicketsCommand(
        short capacity = 100,
        int price = 25_000,
        string ticketName = "Standard")
    {
        return new UpdateTicketsCommand(new TicketDto(ticketName, price, capacity)
        );
    }

    public static PublishEventCommand CreatePublishCommand()
        => new(DefaultEventId);

    public static CancelEventCommand CreateCancelCommand()
        => new(DefaultEventId);

    public static List<TicketDto> CreateSingleTicket(short capacity)
        => new()
        {
            new TicketDto("Standard", 25_000, capacity)
        };
}