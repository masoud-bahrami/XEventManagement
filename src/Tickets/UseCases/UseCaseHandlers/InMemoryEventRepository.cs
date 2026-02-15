using Quantum.ApplicationService;

namespace XEvent.Tickets.Domain.UseCaseHandlers;

public sealed class InMemoryTicketRepository : ITicketsRepository
{
    private readonly IAggregateStore<Tickets, TicketId> _storage;

    public InMemoryTicketRepository(IAggregateStore<Tickets, TicketId> storage)
    {
        _storage = storage;
    }

    public Task Save(Tickets aggregate)
    {
        _storage.Save(aggregate);
        return Task.CompletedTask;
    }

    public Task<Tickets> Load(TicketId id)
        => Task.FromResult(_storage.Get(id));
}