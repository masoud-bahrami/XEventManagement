using Quantum.Domain;

namespace Quantum.ApplicationService;

public interface IEventOutbox
{
    Task Add(IEnumerable<IsADomainEvent> events);
    Task<IReadOnlyCollection<IsADomainEvent>> GetUnpublished();
    Task MarkAsPublished(IEnumerable<IsADomainEvent> events);
}

public sealed class InMemoryEventOutbox : IEventOutbox
{
    private readonly List<IsADomainEvent> _events = new();

    public Task Add(IEnumerable<IsADomainEvent> events)
    {
        _events.AddRange(events);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyCollection<IsADomainEvent>> GetUnpublished()
        => Task.FromResult((IReadOnlyCollection<IsADomainEvent>)_events);

    public Task MarkAsPublished(IEnumerable<IsADomainEvent> events)
    {
        foreach (var e in events)
            _events.Remove(e);

        return Task.CompletedTask;
    }
}
