
using XEvent.AcceptanceTests.Drivers;
using XEvent.Reporting;

namespace XEvent.AcceptanceTests.StepDefinitions
{
    [Binding]
    public class EventManagementStepDefinitions
    {
        private readonly IEventManagementDriver _driver;
        private Table _eventTable;
        private List<EventViewModel> _events;

        public EventManagementStepDefinitions(IEventManagementDriver driver)
        {
            _driver = driver;
        }
        
        [Given(@"a registered user named '([^']*)' exists")]
        public async Task  GivenARegisteredUserNamedAliExists(string name) 
            => await _driver.Login(name);

        [When(@"Ali creates an event with the following details")]
        public async Task WhenAliCreatesAnEventWithTheFollowingDetails(Table table)
        {
            _eventTable = table;
            await _driver.CreateAnEventWith(_eventTable.ToCommand());
        }

        [When(@"He's event will be created in the Draft Status")]
        public async Task WhenHesEventWillBeCreatedInTheDraftStatus()
        {
            await _driver.AssertThatCurrentUserHasOneEventInDraftStatus(_eventTable);
        }

        [Given(@"the following events exist in the system")]
        public async Task GivenTheFollowingEventsExistInTheSystem(Table table) 
            => await _driver.SetupEvents(table);

        [When(@"a user requests the public event list")]
        public async Task WhenAUserRequestsThePublicEventList() => _events =  await _driver.LoadEventsForUsers();

        [Then(@"the user should see exactly the following events")]
        public async Task ThenTheUserShouldSeeExactlyTheFollowingEvents(Table table) 
            => await _driver.AssertThatUserSeesOnlyPublishedEvents(table, _events);
    }
}
