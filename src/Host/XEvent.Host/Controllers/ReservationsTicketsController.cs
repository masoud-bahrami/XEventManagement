using Microsoft.AspNetCore.Mvc;
using XEvent.Reservation.Domain;
using XEvent.Reservation.UseCaseHandlers;

[ApiController]
[Route("api/reservations")]
public class ReservationsTicketsController : ControllerBase
{
    private readonly IReservationTicketServices _reservationTicketServices;

    public ReservationsTicketsController(IReservationTicketServices ticketService) 
        => _reservationTicketServices = ticketService;


    [HttpGet("{reservationId}/tickets")]
    public async Task<IActionResult> GetReservationTickets(long reservationId)
    {
        var pdf =await _reservationTicketServices.GeneratePdfTickets(reservationId);
        return File(pdf, "application/pdf", $"reservation_{reservationId}_tickets.pdf");
    }

    [HttpGet("{reservationId}/attendees/{attendeeId}/ticket")]
    public async Task<IActionResult> GetAttendeeTicket(long reservationId, long attendeeId)
    {
        var pdf = await _reservationTicketServices.GeneratePdfTicket(reservationId, attendeeId);
        return File(pdf, "application/pdf", $"reservation_{reservationId}.pdf");
    }

}