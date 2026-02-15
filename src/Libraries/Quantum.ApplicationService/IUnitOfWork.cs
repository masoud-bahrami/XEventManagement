namespace Quantum.ApplicationService;

public interface IUnitOfWork
{
    Task Commit();
}

public sealed class InMemoryUnitOfWork : IUnitOfWork
{
    public Task Commit()
    {
        // no-op (but hook for transactions later)
        return Task.CompletedTask;
    }
}
