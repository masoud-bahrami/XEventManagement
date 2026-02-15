using Quantum.Domain;

namespace XEvent.Reservation.Domain;

public record ReservationId(long Value):IsAnAggregateRootId(Value);