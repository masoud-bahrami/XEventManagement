using System.Collections;
using Quantum.Domain;
using XEvent.EventManagement.Domain.Contract.Events;

namespace XEvent.EventManagement.Domain;

public sealed class Tickets : IReadOnlyList<Ticket>
{
    private readonly List<Ticket> _tickets = new();

    public Tickets(ICollection<TicketDto> tickets)
    {
        if (tickets == null || tickets.Count == 0)
            throw new ArgumentException("Tickets can not be null or empty");

        foreach (var t in tickets)
        {
            var ticket = CreateTicket(t);
            _tickets.Add(ticket);
        }

        CheckInvariants();
    }

    private Ticket CreateTicket(TicketDto dto)
    {
        EnsureUniqueName(dto.Name);
        var ticket = new Ticket(GenerateTicketId(dto), dto.Name, dto.Price, dto.Capacity);
        return ticket;
    }

    private long GenerateTicketId(TicketDto dto) => dto.GetHashCode();

    public IEnumerator<Ticket> GetEnumerator() => _tickets.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public int Count => _tickets.Count;
    public Ticket this[int index] => _tickets[index];

    private Ticket GetTicketOrThrow(long id)
    {
        var ticket = _tickets.FirstOrDefault(t => t.Id == id);
        return ticket;
    }

    private void EnsureUniqueName(string name, long? excludeId = null)
    {
        if (_tickets.Any(t => t.Name == name && t.Id != excludeId))
            throw new InvalidOperationException($"Ticket with name '{name}' already exists");
    }

    private void CheckInvariants()
    {
        if (_tickets.Select(t => t.Name).Distinct().Count() != _tickets.Count)
            throw new InvalidOperationException("Ticket names should be unique");

        if (_tickets.Select(t => t.Id).Distinct().Count() != _tickets.Count)
            throw new InvalidOperationException("Ticket IDs should be unique");
    }
    
    public void Apply(NewEventTicketIsAddedEvent @event)
    {
        var ticket = new Ticket(GenerateTicketId(@event.Ticket), @event.Ticket.Name, @event.Ticket.Price, @event.Ticket.Capacity);
        EnsureUniqueName(ticket.Name);
        _tickets.Add(ticket);
        CheckInvariants();
    }

    public void Apply(EventTicketIsUpdatedEvent @event)
    {
        var ticket = GetTicketOrThrow(@event.TicketId);
        if (ticket is null)
            throw new InvalidOperationException($"*Ticket with ID {@event.TicketId} not found*");

        EnsureUniqueName(@event.Ticket.Name, ticket.Id);
        ticket.Update(@event.Ticket);
        CheckInvariants();
    }

    public void Apply(ATicketIsRemovedFromEvent @event)
    {
        var ticket = GetTicketOrThrow(@event.TicketId);
        _tickets.Remove(ticket);
    }
}

public sealed class Ticket : IsAnEntity<long>
{
    public decimal Price { get; private set; }
    public string Name { get; private set; }
    public short Capacity { get; private set; }

    public Ticket(long id, string name, decimal price, short capacity) : base(id)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or empty");
        if (capacity <= 0)
            throw new ArgumentException("Capacity must be greater than zero");
        if (price < 0)
            throw new ArgumentException("Price cannot be negative");

        Name = name.Trim();
        Price = price;
        Capacity = capacity;
    }

    public void Update(TicketDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Name cannot be null or empty");
        if (dto.Capacity <= 0)
            throw new ArgumentException("Capacity must be greater than zero");
        if (dto.Price < 0)
            throw new ArgumentException("Price cannot be negative");

        Name = dto.Name.Trim();
        Price = dto.Price;
        Capacity = dto.Capacity;
    }
}
