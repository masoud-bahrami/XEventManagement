using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quantum.ApplicationService;
using XEvent.Tickets.Domain.UseCaseHandlers;

public static class TicketBootstrapper
{
    public static void Run(
        IServiceCollection services,
        ConfigurationManager configuration,
        BootstrappingMode mode)
    {
        // Application
        services.AddScoped<ITicketServices, TicketService>();
        
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

        services.AddSingleton<IEventOutbox, InMemoryEventOutbox>();

        services.AddScoped<ITicketsRepository, InMemoryTicketRepository>();
        services.AddScoped<IEventOutbox, InMemoryEventOutbox>();

        services.AddScoped<IUnitOfWork, InMemoryUnitOfWork>();
    }

    private static void RegisterProduction(
        IServiceCollection services,
        ConfigurationManager configuration)
    {
        throw new NotImplementedException("Production wiring not implemented yet");
    }
}