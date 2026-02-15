using XEvent.EventManagement.Domain.Contract.Commands;
using XEvent.Reporting;

namespace XEvent.AcceptanceTests.Drivers;

public interface IEventManagementDriver
{
    Task Login(string name);
    Task CreateAnEventWith(CreateNewEventCommand cmd);
    Task AssertThatCurrentUserHasOneEventInDraftStatus(Table eventTable);
        Task SetupEvents(Table table);
        Task<List<EventViewModel>> LoadEventsForUsers();
        Task AssertThatUserSeesOnlyPublishedEvents(Table table, List<EventViewModel> events);
}