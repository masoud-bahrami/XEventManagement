using Quantum.Domain;

public class ReservedAttendee : IsAnEntity<long>
{
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public ReservationStatus Status { get; private set; } = ReservationStatus.Pending;

    public ReservedAttendee(long id,string firstName, string lastName)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Status = ReservationStatus.Pending;
    }

    public void Approve() => Status = ReservationStatus.Approved;
    public void Cancel() => Status = ReservationStatus.Cancelled;
}

public enum ReservationStatus
{
    Pending,
    Approved,
    Cancelled
}