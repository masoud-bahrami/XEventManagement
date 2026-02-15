using System.Collections.Concurrent;
using Quantum.Domain;

namespace Quantum.ApplicationService;

public interface IAggregateStore<TAggregate, TId>
    where TAggregate : IsAnAggregateRoot<TAggregate, TId>
    where TId : IsAnAggregateRootId
{
    void Save(TAggregate aggregate);
    TAggregate? Get(TId id);
    bool Exists(TId id);
    IReadOnlyCollection<TAggregate> GetAll();
}


public sealed class InMemoryAggregateStore<TAggregate, TId>
    : IAggregateStore<TAggregate, TId>
    where TAggregate : IsAnAggregateRoot<TAggregate, TId>
    where TId : IsAnAggregateRootId
{
    private readonly ConcurrentDictionary<TId, TAggregate> _store = new();

    public void Save(TAggregate aggregate)
    {
        _store.AddOrUpdate(
            aggregate.Id,
            aggregate,
            (_, existing) =>
            {
                // Optimistic concurrency (simple)
                if (aggregate.Version <= existing.Version)
                    throw new InvalidOperationException("Concurrency conflict");

                return aggregate;
            });
    }

    public TAggregate? Get(TId id)
    {
        _store.TryGetValue(id, out var aggregate);
        return aggregate;
    }

    public bool Exists(TId id)
        => _store.ContainsKey(id);

    public IReadOnlyCollection<TAggregate> GetAll()
        => _store.Values.ToList().AsReadOnly();
}
