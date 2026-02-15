using Microsoft.AspNetCore.Mvc;
using XEvent.Reservation.Domain;
using XEvent.Reservation.UseCaseHandlers;

[ApiController]
[Route("api/reservations")]
public class ReservationsController : ControllerBase
{
    private readonly IReservationServices _services;

    public ReservationsController(IReservationServices services)
    {
        _services = services;
    }

    [HttpPost]
    public async Task<IActionResult> Reserve(
        [FromBody] CreateReservationCommand command,
        CancellationToken ct)
    {
        var id = await _services.Reserve(command, ct);
        return Ok(new { reservationId = id.Value });
    }
}