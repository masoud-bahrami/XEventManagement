namespace XEvent.Reporting;

public interface IEventRepository
{
    Task<IReadOnlyCollection<EventViewModel>> LoadAll(long userId);
    Task<IReadOnlyCollection<EventViewModel>> LoadAll();
}