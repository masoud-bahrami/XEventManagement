using Quantum.Domain;
using XEvent.EventManagement.Domain.Contract;
using XEvent.EventManagement.Domain.Contract.Commands;
using XEvent.EventManagement.Domain.Contract.Events;

namespace XEvent.EventManagement.Domain;

public partial class Event : IsAnAggregateRoot<Event, EventId>
{
    public long Owner { get; private set; }
    public string Name { get; private set; }
    public Tickets Tickets { get; private set; }
    public EventDateRange Duration { get; private set; }
    public Status Status { get; private set; }
    public Event(long Id, long owner, CreateNewEventCommand cmd)
    {
        foreach (var domainEvent in cmd.EventuateTo(Id, owner))
        {
            RaiseEvent(domainEvent);
        }
    }
    public void Handle(AddNewTicketCommand command)
    {
        if (Status == Status.Cancelled)
            throw new InvalidOperationException("Cannot update a published event");

        foreach (var domainEvent in command.EventuateTo(Id.value))
        {
            RaiseEvent(domainEvent);
        }
    }
    
    public void Handle(DeleteTicketCommand command)
    {
        if (Status == Status.Cancelled)
            throw new InvalidOperationException("Cannot update a published event");
        
        foreach (var domainEvent in command.EventuateTo(Id.value))
        {
            RaiseEvent(domainEvent);
        }
    }
    
    public void Handle(UpdateEventCommand command)
    {
        if (Status == Status.Published)
            throw new InvalidOperationException("Cannot update a published event");

        foreach (var domainEvent in command.EventuateTo())
        {
            RaiseEvent(domainEvent);
        }
    }
    

    public void Handle(long ticketId, UpdateTicketsCommand command)
    {
        if (Status == Status.Cancelled)
            throw new InvalidOperationException("Cannot update a published event");

        foreach (var domainEvent in command.EventuateTo(Id.value, ticketId))
        {
            RaiseEvent(domainEvent);
        }
    }

    public void Handle(PublishEventCommand command)
    {
        if (Status == Status.Published) return;

        Status = Status.Published;
        RaiseEvent(new EventPublished(Id.Value));
    }

    public void Handle(MoveEventToDraftCommand command)
    {
        if (Status == Status.Draft) return;

        Status = Status.Draft;
        RaiseEvent(new EventMovedToDraft(Id.Value));
    }

    public void Handle(CancelEventCommand command)
    {
        if (Status == Status.Cancelled) return;

        Status = Status.Cancelled;
        RaiseEvent(new EventCancelled(Id.Value));
    }

    public void Handle(DeleteEventCommand command)
    {
        RaiseEvent(new EventDeleted(Id.Value));
    }
}

public partial class Event
{

    public Event(ICollection<IsADomainEvent> events)
    {
        foreach (var isADomainEvent in events)
        {
            Apply(isADomainEvent);
        }
    }
    protected override void Apply(IsADomainEvent domainEvent)
    {
        Apply((dynamic)domainEvent);
    }

    private void Apply(EventCreated e)
    {
        Id = new EventId(e.EventId);
        Owner = e.Owner;
        Name = e.Name;
        Duration = new EventDateRange(e.StartDate, e.EndDate);
        Status = Status.Draft;
    }

    private void Apply(EventTicketsIsSet e) 
        => Tickets = new Tickets(e.Tickets);    

    private void Apply(EventUpdated e)
    {
        Name = e.Name;
        Duration = new EventDateRange(e.StartDate, e.EndDate);
    }

    private void Apply(EventPublished e) => Status = Status.Published;
    private void Apply(EventMovedToDraft e) => Status = Status.Draft;
    private void Apply(EventCancelled e) => Status = Status.Cancelled;


    public void Apply(NewEventTicketIsAddedEvent @event)
    {
        Tickets.Apply(@event);
    }
    
    public void Apply(EventTicketIsUpdatedEvent @event)
    {
        Tickets.Apply(@event);
    }

    public void Apply(ATicketIsRemovedFromEvent @event)
    {
        Tickets.Apply(@event);
    }

    private void Apply(EventDeleted e)
    {
        // Optionally mark a flag or remove from repository outside aggregate
    }
}