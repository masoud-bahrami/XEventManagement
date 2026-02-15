namespace Quantum.ApplicationService;

public interface IIdGenerator
{
    long NextId();
}

public sealed class SequentialIdGenerator : IIdGenerator
{
    private long _current = 0;
    public long NextId() => Interlocked.Increment(ref _current);
}
