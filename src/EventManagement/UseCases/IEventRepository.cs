using XEvent.EventManagement.Domain;

namespace XEvent.EventManagement.UseCaseHandlers;

public interface IEventRepository
{
    Task Save(Event aggregate);
    Task<Event?> Load(EventId id);
    Task<IReadOnlyCollection<Event>> LoadAll();
}