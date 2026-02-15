using Microsoft.AspNetCore.Mvc;
using XEvent.Tickets.Domain.Contracts.Commands;
using XEvent.Tickets.Domain.UseCaseHandlers;

namespace XEvent.Host.Controllers;

[ApiController]
[Route("api/{id}/[Controller]")]
public class TicketsController : ControllerBase
{
    private readonly ITicketServices _services;

    public TicketsController(ITicketServices services) => _services = services;

    [HttpPut]
    public async Task<IActionResult> AddTicket(long id, [FromBody] AddNewTicketCommand command)
    {
        await _services.Handle(id, command);
        return Ok();
    }

    [HttpPut("{ticketId}")]
    public async Task<IActionResult> UpdateTicket(long id, long ticketId,
        [FromBody] UpdateTicketsCommand command)
    {
        await _services.Handle(id, ticketId, command);
        return Ok();
    }

    [HttpDelete("{ticketId}")]
    public async Task<IActionResult> DeleteTicket(long id, long ticketId)
    {
        await _services.DeleteTicket(id, ticketId);
        return Ok();
    }
}