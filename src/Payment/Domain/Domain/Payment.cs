using Quantum.Domain;
using XEvent.Reservation.Domain;

namespace XEvent.Payment.Domain;

public class Payment : IsAnAggregateRoot<Payment, PaymentId>
{
    public PaymentId Id { get; private set; }
    public ReservationId ReservationId { get; private set; }
    public decimal Amount { get; private set; }
    public PaymentStatus Status { get; private set; }

    public Payment(PaymentId id, ReservationId reservationId, decimal amount)
    {
        Id = id;
        ReservationId = reservationId;
        Amount = amount;
        Status = PaymentStatus.Pending;

        RaiseEvent(new PaymentCreatedEvent(Id, ReservationId, Amount));
    }

    public void MarkSucceeded()
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException("Payment cannot be marked succeeded");

        RaiseEvent(new PaymentSucceededEvent(Id, ReservationId));
    }

    public void MarkFailed()
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException("Payment cannot be marked failed");

        RaiseEvent(new PaymentFailedEvent(Id, ReservationId));
    }

    protected override void Apply(IsADomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case PaymentCreatedEvent e:
                Apply(e);
                break;
            case PaymentSucceededEvent e:
                Apply(e);
                break;
            case PaymentFailedEvent e:
                Apply(e);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(domainEvent), $"Unknown event type: {domainEvent.GetType().Name}");
        }
    }

    private void Apply(PaymentCreatedEvent e)
    {
        Id = e.PaymentId;
        ReservationId = e.ReservationId;
        Amount = e.Amount;
        Status = PaymentStatus.Pending;
    }

    private void Apply(PaymentSucceededEvent e)
    {
        Status = PaymentStatus.Succeeded;
    }

    private void Apply(PaymentFailedEvent e)
    {
        Status = PaymentStatus.Failed;
    }

    public record PaymentCreatedEvent(PaymentId PaymentId, ReservationId ReservationId, decimal Amount) : IsADomainEvent;
    public record PaymentSucceededEvent(PaymentId PaymentId, ReservationId ReservationId) : IsADomainEvent;
    public record PaymentFailedEvent(PaymentId PaymentId, ReservationId ReservationId) : IsADomainEvent;
}
