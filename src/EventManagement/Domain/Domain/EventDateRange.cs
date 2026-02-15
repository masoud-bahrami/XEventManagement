using Quantum.Domain;

namespace XEvent.EventManagement.Domain;

public sealed class EventDateRange : IsAValueObject
{
    public DateTime Start { get; }
    public DateTime End { get; }

    public EventDateRange(DateTime start, DateTime end)
    {
        if (end <= start)
            throw new ArgumentException("End date must be after start date");

        Start = start;
        End = end;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Start;
        yield return End;
    }
}