using XEvent.Reservation.Domain;

namespace XEvent.Reservation.UseCaseHandlers;
public interface ITicketCapacityStore
{
    Task<bool> TryReserveAsync(ReservationId reservationId,
        EventId eventId,
        TicketTypeId ticketTypeId,
        int quantity,
        TimeSpan ttl,
        CancellationToken ct);

    Task UpdateCapacityAsync(
        EventId eventId,
        TicketTypeId ticketTypeId,
        int newCapacity,
        CancellationToken ct);

    Task ReleaseAsync(
        ReservationId reservationId,
        EventId eventId,
        TicketTypeId ticketTypeId,
        CancellationToken ct);

    Task ConfirmAsync(
        ReservationId reservationId,
        EventId eventId,
        TicketTypeId ticketTypeId,
        CancellationToken ct);
}
