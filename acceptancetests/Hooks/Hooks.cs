using BoDi;
using Microsoft.AspNetCore.Mvc.Testing;
using XEvent.AcceptanceTests.Drivers;
using XEvent.Host.Controllers;

namespace XEvent.AcceptanceTests.Hooks;

[Binding]
public sealed class Hooks
{
    private readonly ScenarioContext _scenarioContext;
    private readonly IObjectContainer _objectContainer;

    public Hooks(ScenarioContext scenarioContext, IObjectContainer objectContainer)
    {
        _scenarioContext = scenarioContext;
        _objectContainer = objectContainer;
    }

    [BeforeScenario]
    public void CreateApplication()
    {
        Environment.SetEnvironmentVariable("BootstrappingMode", "Test");

        var application = new WebApplicationFactory<EventsController>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                });
            });

        var httpClient = application.CreateClient();

        _objectContainer.RegisterInstanceAs(httpClient, typeof(HttpClient));
        _objectContainer.RegisterTypeAs<EventManagementTestFramework, EventManagementTestFramework>();

        _objectContainer.RegisterInstanceAs(application.Services, typeof(IServiceProvider));

        _scenarioContext.Add("serviceProvider", application.Services);
    }

    [BeforeScenario]
    public void BeforeScenarioWithTag()
    {
        _objectContainer.RegisterTypeAs<EventManagementApiDriver, IEventManagementDriver>();
    }
}