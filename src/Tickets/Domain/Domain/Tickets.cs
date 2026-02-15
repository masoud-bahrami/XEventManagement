using System.Collections;
using Quantum.Domain;
using XEvent.Tickets.Domain.Contracts;
using XEvent.Tickets.Domain.Contracts.Commands;
using XEvent.Tickets.Domain.Contracts.Events;

namespace XEvent.Tickets.Domain;

public sealed class Tickets :
    IsAnAggregateRoot<Tickets, TicketId>
    , IReadOnlyList<Ticket>
{
    private readonly List<Ticket> _tickets = new();

    public Tickets(TicketId ticketId, ICollection<TicketDto> tickets)
    {
        Id = ticketId;

        if (tickets == null || tickets.Count == 0)
            throw new ArgumentException("Tickets can not be null or empty");

        foreach (var t in tickets)
        {
            var ticket = CreateTicket(t);
            _tickets.Add(ticket);
        }

        CheckInvariants();
    }

    public void Handle(long ticketId, UpdateTicketsCommand cmd)
    {
        foreach (var isADomainEvent in cmd.EventuateTo(Id.Value, ticketId))
            RaiseEvent(isADomainEvent);
    }
    
    public void Handle(DeleteTicketCommand cmd)
    {
        foreach (var isADomainEvent in cmd.EventuateTo(Id.Value))
            RaiseEvent(isADomainEvent);
    }
    
    public void Handle(AddNewTicketCommand cmd)
    {
        foreach (var isADomainEvent in cmd.EventuateTo(Id.Value, cmd.Ticket.GetHashCode()))
            RaiseEvent(isADomainEvent);
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

    protected override void Apply(IsADomainEvent domainEvent)
    {
        Apply((dynamic)domainEvent);
    }
}