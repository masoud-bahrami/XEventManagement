using Quantum.Domain;

public class ReservedAttendee : IsAnEntity<long>
{
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public AttendeeStatus Status { get; private set; } = AttendeeStatus.Pending;

    public ReservedAttendee(long id,string firstName, string lastName)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
    }

    public void MarkCheckedIn() => Status = AttendeeStatus.CheckedIn;
    public void Cancel() => Status = AttendeeStatus.Cancelled;
}

public enum AttendeeStatus
{
    Pending,
    CheckedIn,
    Cancelled,
    NoShow
}