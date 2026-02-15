using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Quantum.ApplicationService;
using SpecFlow.Internal.Json;
using XEvent.EventManagement.Domain;
using XEvent.EventManagement.Domain.Contract.Commands;
using XEvent.EventManagement.UseCaseHandlers;
using XEvent.Reporting;

namespace XEvent.AcceptanceTests.Drivers;

public class EventManagementApiDriver : IEventManagementDriver
{
    private readonly HttpClient _httpClient;
    private readonly IServiceProvider _provider;
    private readonly EventManagementTestFramework _framework;
    private const string ApiBaseAddress = "api/events";
    public EventManagementApiDriver(HttpClient httpClient, System.IServiceProvider provider, EventManagementTestFramework framework)
    {
        _httpClient = httpClient;
        _provider = provider;
        _framework = framework;
    }

    public async Task Login(string name)
    {
        var scope = _provider.CreateScope();
        var service = scope.ServiceProvider.GetService<ICurrentUser>();
        if(service is FakeCurrentUser user)
            user.SetCurrentUser(1);
    }

    public async Task CreateAnEventWith(CreateNewEventCommand cmd)
    {
        var httpResponseMessage = await _httpClient.PostAsync(ApiBaseAddress, cmd.ToHttpCommand());
        httpResponseMessage.EnsureSuccessStatusCode();
    }

    public async Task SetupEvents(Table table)
    {
        await _framework.INeedToHaveTheseEvents(table);
    }

    public async Task<List<EventViewModel>> LoadEventsForUsers()
    {
        var httpResponseMessage = await _httpClient.GetAsync(ApiBaseAddress+ "/public");
        httpResponseMessage.EnsureSuccessStatusCode();

        var json = await httpResponseMessage.Content.ReadAsStringAsync();
        var events = json.FromJson<PagedResult<EventViewModel>>();

        return events.Items.ToList();
    }

    public async Task AssertThatCurrentUserHasOneEventInDraftStatus(Table eventTable)
    {
        // Arrange
        var expected = eventTable.Rows[0];

        var expectedName = expected["Name"];
        var expectedCapacity = short.Parse(expected["Capacity"]);

        // Act
        var httpResponseMessage = await _httpClient.GetAsync(ApiBaseAddress);
        httpResponseMessage.EnsureSuccessStatusCode();

        var json = await httpResponseMessage.Content.ReadAsStringAsync();
        var events = json.FromJson<List<EventViewModel>>();

        // Assert - measurable & concrete
        events.Should().HaveCount(1);

        var ev = events.Single();

        ev.Name.Should().Be(expectedName);
        
        ev.Status.Should().Be(Status.Draft); // Draft
    }
    public async Task AssertThatUserSeesOnlyPublishedEvents(Table table, List<EventViewModel> events)
    {
        // Arrange - 
        var expectedNames = table.Rows.Select(r => r["Name"]).ToList();



        events.Should().HaveCount(expectedNames.Count);
        
        var actualNames = events.Select(e => e.Name).OrderBy(n => n).ToList();
        expectedNames = expectedNames.OrderBy(n => n).ToList();

        actualNames.Should().Equal(expectedNames);
    }

}