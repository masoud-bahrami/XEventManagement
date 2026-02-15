using Quantum.ApplicationService;
using XEvent.Reservation.Domain;
using XEvent.Reservation.UseCaseHandlers;

namespace XEvent.Reservation.ACL;

public sealed class ReservationPaymentProjection :
    IWantToProject<Payment.Domain.Payment.PaymentSucceededEvent>,
    IWantToProject<Payment.Domain.Payment.PaymentFailedEvent>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly ITicketCapacityStore _capacityStore;

    public ReservationPaymentProjection(
        IReservationRepository reservationRepository,
        ITicketCapacityStore capacityStore)
    {
        _reservationRepository = reservationRepository;
        _capacityStore = capacityStore;
    }

    // Payment succeeded -> confirm reservation
    public async Task On(Payment.Domain.Payment.PaymentSucceededEvent @event, CancellationToken ct)
    {
        var reservation = await _reservationRepository.GetAsync(
            new ReservationId(@event.ReservationId.Value), ct);

        if (reservation == null)
            return; // or throw

        reservation.MarkConfirmed();

        await _reservationRepository.UpdateAsync(reservation, ct);

        // Confirm capacity in Redis (remove temporary reservation)
        await _capacityStore.ConfirmAsync(
            new ReservationId(@event.ReservationId.Value),
            new EventId(reservation.EventId.Value),
            new TicketTypeId(reservation.TicketTypeId.Value),
            ct);
    }

    // Payment failed -> cancel reservation
    public async Task On(Payment.Domain.Payment.PaymentFailedEvent @event, CancellationToken ct)
    {
        var reservation = await _reservationRepository.GetAsync(
            new ReservationId(@event.ReservationId.Value), ct);

        if (reservation == null)
            return;

        reservation.MarkCancelled();

        await _reservationRepository.UpdateAsync(reservation, ct);

        // Release capacity in Redis
        await _capacityStore.ReleaseAsync(
            new ReservationId(@event.ReservationId.Value),
            new EventId(reservation.EventId.Value),
            new TicketTypeId(reservation.TicketTypeId.Value),
            ct);
    }
}
