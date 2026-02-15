using Quantum.ApplicationService;
using XEvent.EventManagement.Domain;
using XEvent.EventManagement.Domain.Contract.Events;

namespace XEvent.Reporting;

public sealed class InMemoryEventRepository : IEventRepository
{
    private readonly IAggregateStore<Event, EventId> _storage;

    public InMemoryEventRepository(IAggregateStore<Event, EventId> storage)
    {
        _storage = storage;
    }

    public async Task<IReadOnlyCollection<EventViewModel>> LoadAll(long userId)
    {
        var events = _storage.GetAll().Where(e => e.Owner == userId).ToList();
        
        return ToEventViewModel(events);

    }

    public async Task<IReadOnlyCollection<EventViewModel>> LoadAll()
    {
        var events = _storage.GetAll()
            .Where(e => e.Status == Status.Published && e.Duration.End <= DateTime.Now);

        return ToEventViewModel(events);
    }

    private static IReadOnlyCollection<EventViewModel> ToEventViewModel(IEnumerable<Event> events)
    {
        return events.Select(e => new EventViewModel(
                e.Id.Value,
                e.Owner,
                e.Name,
                ToTickets(e.Tickets),
                e.Duration.Start,
                e.Duration.End,
                e.Status
            ))
            .ToArray();
    }

    private static ICollection<TicketDto> ToTickets(Tickets tickets)
    {
        return tickets.Select(a => new TicketDto(a.Name, a.Price, a.Capacity)).ToList();
    }
}