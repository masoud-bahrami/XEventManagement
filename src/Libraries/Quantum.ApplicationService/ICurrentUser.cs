namespace Quantum.ApplicationService;

public interface ICurrentUser
{
    long UserId { get; }
}

public class FakeCurrentUser : ICurrentUser
{
    private static long _userId;

    public void SetCurrentUser(long userId)
    {
        _userId = userId;}
    public long UserId => _userId;
}
