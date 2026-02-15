namespace Quantum.Domain;

public abstract class IsAnAggregateRoot<TAggregate, TId>
    where TId : IsAnAggregateRootId
    where TAggregate : IsAnAggregateRoot<TAggregate, TId>
{
    private readonly List<IsADomainEvent> _uncommittedEvents = new();

    public TId Id { get; protected set; }
    public int Version { get; private set; } = -1;

    protected IsAnAggregateRoot() { }

    protected void RaiseEvent(IsADomainEvent domainEvent)
    {
        Apply(domainEvent);
        _uncommittedEvents.Add(domainEvent);
        Version++;
    }

    protected abstract void Apply(IsADomainEvent domainEvent);

    public IReadOnlyCollection<IsADomainEvent> GetUncommittedEvents()
        => _uncommittedEvents.AsReadOnly();

    public void ClearUncommittedEvents()
        => _uncommittedEvents.Clear();

    public void LoadFromHistory(IEnumerable<IsADomainEvent> history)
    {
        foreach (var domainEvent in history.OrderBy(e => e.Version))
        {
            Apply(domainEvent);
            Version = domainEvent.Version;
        }
    }

    #region Equality

    public override bool Equals(object obj)
    {
        if (obj is not TAggregate other)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (Id is null || other.Id is null)
            return false;

        return Id.Equals(other.Id);
    }

    public override int GetHashCode()
        => Id?.GetHashCode() ?? base.GetHashCode();

    public static bool operator ==(
        IsAnAggregateRoot<TAggregate, TId> left,
        IsAnAggregateRoot<TAggregate, TId> right)
        => Equals(left, right);

    public static bool operator !=(
        IsAnAggregateRoot<TAggregate, TId> left,
        IsAnAggregateRoot<TAggregate, TId> right)
        => !Equals(left, right);

    #endregion
}