namespace Quantum.Domain;

public abstract class IsAnEntity<TId>
{
    public TId Id { get; protected set; }

    protected IsAnEntity(TId id)
    {
        Id = id;
    }

    protected IsAnEntity() { }

    public override bool Equals(object obj)
    {
        if (obj is null || obj.GetType() != GetType())
            return false;

        var other = (IsAnEntity<TId>)obj;

        if (ReferenceEquals(this, other))
            return true;

        if (Id is null || other.Id is null)
            return false;

        return Id.Equals(other.Id);
    }

    public override int GetHashCode()
        => Id?.GetHashCode() ?? base.GetHashCode();
}