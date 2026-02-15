using Quantum.ApplicationService;
using XEvent.EventManagement.Domain;

namespace XEvent.EventManagement.UseCaseHandlers;

public sealed class InMemoryEventRepository : IEventRepository
{
    private readonly IAggregateStore<Event, EventId> _storage;

    public InMemoryEventRepository(IAggregateStore<Event, EventId> storage)
    {
        _storage = storage;
    }

    public Task Save(Event aggregate)
    {
        _storage.Save(aggregate);
        return Task.CompletedTask;
    }

    public Task<Event?> Load(EventId id)
        => Task.FromResult(_storage.Get(id));

    public Task<IReadOnlyCollection<Event>> LoadAll()
        => Task.FromResult(_storage.GetAll());

}