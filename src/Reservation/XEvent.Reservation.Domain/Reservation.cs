using Quantum.Domain;

namespace XEvent.Reservation.Domain;

public partial class Reservation : IsAnAggregateRoot<Reservation, ReservationId>
{
    public ReservationId Id { get; private set; }
    public EventId EventId { get; private set; }
    public TicketTypeId TicketTypeId { get; private set; }
    public BuyerId BuyerId { get; private set; }
    public int Quantity => Attendees.Count;
    public DateTime ExpiresAt { get; private set; }
    public ReservationStatus Status { get; private set; }

    private readonly List<ReservedAttendee> _attendees = new();
    public IReadOnlyCollection<ReservedAttendee> Attendees => _attendees.AsReadOnly();

    private Reservation() { }

    public Reservation(
        ReservationId id,
        BuyerId buyerId,
        CreateReservationCommand cmd,
        TimeSpan ttl)
    {
        if (!cmd.Attendees.Any())
            throw new Exception("Invalid quantity");

        Id = id;
        EventId = new EventId(cmd.EventId);
        TicketTypeId = new TicketTypeId(cmd.TicketTypeId);
        BuyerId = buyerId;
        Status = ReservationStatus.Pending;
        ExpiresAt = DateTime.UtcNow.Add(ttl);

        _attendees.AddRange(cmd.Attendees.Select(a => 
            new ReservedAttendee(a.GetHashCode(), a.FirstName, a.LastName)));

        RaiseEvent(new ReservationCreatedEvent(Id, EventId, TicketTypeId, BuyerId, Quantity, ExpiresAt));
    }

    public void MarkConfirmed()
    {
        if (Status != ReservationStatus.Pending)
            throw new InvalidOperationException("Reservation cannot be confirmed");
        RaiseEvent(new ReservationConfirmedEvent(Id));
    }

    public void MarkCancelled()
    {
        if (Status != ReservationStatus.Pending)
            throw new InvalidOperationException("Reservation cannot be cancelled");

        RaiseEvent(new ReservationCancelledEvent(Id));
    }

    protected override void Apply(IsADomainEvent domainEvent)
    {
        Apply((dynamic)domainEvent);
    }

    private void Apply(ReservationCreatedEvent @event)
    {
        Id = @event.ReservationId;
        EventId = @event.EventId;
        TicketTypeId = @event.TicketTypeId;
        BuyerId = @event.BuyerId;
        Status = ReservationStatus.Pending;
        ExpiresAt = @event.ExpiresAt;
    }

    private void Apply(ReservationConfirmedEvent @event) 
        => Status = ReservationStatus.Confirmed;

    private void Apply(ReservationCancelledEvent @event)
    {
        Status = ReservationStatus.Cancelled;
    }

    // Domain Events
    public record ReservationCreatedEvent(
        ReservationId ReservationId,
        EventId EventId,
        TicketTypeId TicketTypeId,
        BuyerId BuyerId,
        int Quantity,
        DateTime ExpiresAt) : IsADomainEvent;

    public record ReservationConfirmedEvent(ReservationId ReservationId) : IsADomainEvent;
    public record ReservationCancelledEvent(ReservationId ReservationId) : IsADomainEvent;
}
