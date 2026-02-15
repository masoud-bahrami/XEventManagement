using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quantum.ApplicationService;

namespace XEvent.Reservation.Bootstrapper;

public class ReservationBootstrapper
{
    public static void Run(IServiceCollection builderServices, ConfigurationManager builderConfiguration, BootstrappingMode bootstrappingMode)
    {
        //IReservationTicketServices , ReservationTicketServices
    }
}