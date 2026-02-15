using Microsoft.AspNetCore.Mvc;
using XEvent.CheckIn.UseCaseHandlers;
using XEvent.Reservation.Domain;

[ApiController]
[Route("api/[Controller]")]
public class CheckInController : ControllerBase
{
    private readonly ICheckInService _service;

    public CheckInController(ICheckInService service)
    {
        _service = service;
    }

    [HttpPost("{reservationId:long}/attendee/{attendeeId:long}")]
    public async Task<IActionResult> CheckIn(long reservationId, long attendeeId, CancellationToken ct  =default)
    {
        await _service.CheckInAsync(new ReservationId(reservationId), attendeeId, ct);
        return Ok(new { ReservationId = reservationId, AttendeeId = attendeeId, CheckedInAt = DateTime.UtcNow });
    }

    [HttpGet("{reservationId:long}")]
    public async Task<IActionResult> GetAll(long reservationId, CancellationToken ct)
    {
        var records = await _service.GetAllByReservationAsync(new ReservationId(reservationId), ct);
        return Ok(records);
    }
}