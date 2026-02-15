using XEvent.Reservation.Domain;

namespace XEvent.CheckIn.Domain;


public enum AttendeeStatus
{
    Pending,
    CheckedIn,
    Cancelled,
    NoShow
}

public class CheckInRecord
{
    public ReservationId ReservationId { get; }
    public long AttendeeId { get; }
    public AttendeeStatus Status { get; private set; }
    public DateTime CheckedInAt { get; private set; }

    public CheckInRecord(ReservationId reservationId, long attendeeId, DateTime checkedInAt)
    {
        ReservationId = reservationId;
        AttendeeId = attendeeId;
        CheckedInAt = checkedInAt;
        Status = AttendeeStatus.Pending;
    }

    public void MarkCheckedIn(DateTime time)
    {
        Status= AttendeeStatus.CheckedIn;
        CheckedInAt = time;
    }
}