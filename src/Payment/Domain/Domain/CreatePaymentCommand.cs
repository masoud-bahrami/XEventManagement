using XEvent.Reservation.Domain;

namespace XEvent.Payment.Domain;

public record CreatePaymentCommand(ReservationId ReservationId, decimal Amount);