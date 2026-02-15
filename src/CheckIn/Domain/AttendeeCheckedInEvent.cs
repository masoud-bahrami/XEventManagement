using Quantum.Domain;
using XEvent.Reservation.Domain;

namespace XEvent.CheckIn.Domain;

public record AttendeeCheckedInEvent(ReservationId ReservationId, long AttendeeId, DateTime CheckedInAt) : IsADomainEvent;