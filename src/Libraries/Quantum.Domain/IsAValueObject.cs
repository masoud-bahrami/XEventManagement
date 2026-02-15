namespace Quantum.Domain;

public abstract class IsAValueObject
{
    protected abstract IEnumerable<object> GetEqualityComponents();

    public override bool Equals(object obj)
    {
        if (obj is null || obj.GetType() != GetType())
            return false;

        var other = (IsAValueObject)obj;

        return GetEqualityComponents()
            .SequenceEqual(other.GetEqualityComponents());
    }

    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Aggregate(1, (current, obj) =>
            {
                unchecked
                {
                    return current * 23 + (obj?.GetHashCode() ?? 0);
                }
            });
    }

    public static bool operator ==(IsAValueObject a, IsAValueObject b)
        => a is null ? b is null : a.Equals(b);

    public static bool operator !=(IsAValueObject a, IsAValueObject b)
        => !(a == b);
}