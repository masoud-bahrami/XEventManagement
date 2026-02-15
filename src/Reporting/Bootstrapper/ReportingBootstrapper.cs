using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quantum.ApplicationService;

namespace XEvent.Reporting.Bootstrapper;

public static class ReportingBootstrapper
{
    public static void Run(
        IServiceCollection services,
        ConfigurationManager configuration,
        BootstrappingMode mode)
    {
        // Application
        services.AddScoped<IEventQueryManagementServices, EventQueryManagementService>();

        // Domain Services
        services.AddScoped<ICurrentUser, FakeCurrentUser>();
        
        // Infrastructure (mode-based)
        switch (mode)
        {
            case BootstrappingMode.Test:
                RegisterInMemory(services);
                break;

            case BootstrappingMode.Production:
                RegisterProduction(services, configuration);
                break;
        }
    }

    private static void RegisterInMemory(IServiceCollection services)
    {
        services.AddSingleton(typeof(IAggregateStore<,>),
            typeof(InMemoryAggregateStore<,>));
        

        services.AddScoped<IEventRepository, InMemoryEventRepository>();
        
    }

    private static void RegisterProduction(
        IServiceCollection services,
        ConfigurationManager configuration)
    {
        throw new NotImplementedException("Production wiring not implemented yet");
    }
}