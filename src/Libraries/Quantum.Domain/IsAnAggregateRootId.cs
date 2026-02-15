namespace Quantum.Domain;

public abstract record IsAnAggregateRootId
{
    public long Value { get; }

    protected IsAnAggregateRootId(long value)
    {
        if (value == 0)
            throw new ArgumentException("AggregateRootId cannot be empty");

        Value = value;
    }

    public override string ToString() => Value.ToString();
}