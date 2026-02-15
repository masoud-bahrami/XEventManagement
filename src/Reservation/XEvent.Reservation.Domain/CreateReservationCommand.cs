namespace XEvent.Reservation.Domain;

public record CreateReservationCommand(
    long EventId,
    long TicketTypeId,
    List<ReservedAttendee> Attendees);