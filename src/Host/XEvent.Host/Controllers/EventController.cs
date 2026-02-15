using Microsoft.AspNetCore.Mvc;
using XEvent.EventManagement.Domain.Contract.Commands;
using XEvent.EventManagement.UseCaseHandlers;
using XEvent.Reporting;

namespace XEvent.Host.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly IEventManagementServices _services;
    private readonly IEventQueryManagementServices _queryServices;

    public EventsController(IEventManagementServices services, IEventQueryManagementServices queryServices)
    {
        _services = services;
        _queryServices = queryServices;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return Ok(await _queryServices.All());
    }

    [HttpGet("public")]
    public async Task<IActionResult> GetPublic(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? nameFilter = null,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null)
    {
        return Ok(await _queryServices.AllPublicEvents("api/events", page, pageSize, nameFilter, from, to));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody]CreateNewEventCommand command)
    {
        await _services.Handle(command);
        return Ok();
    }

    [HttpPut("{Id}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateEventCommand command)
    {
        await _services.Handle(id, command);
        return Ok();
    }
    
    [HttpPut("{Id}/tickets")]
    public async Task<IActionResult> AddTicket(long id, [FromBody] AddNewTicketCommand command)
    {
        await _services.Handle(id, command);
        return Ok();
    }

    [HttpPut("{Id}/tickets/{ticketId}")]
    public async Task<IActionResult> UpdateTicket(long id, long ticketId,
        [FromBody] UpdateTicketsCommand command)
    {
        await _services.Handle(id, ticketId, command);
        return Ok();
    }

    [HttpDelete("{Id}/tickets/{ticketId}")]
    public async Task<IActionResult> DeleteTicket(long id,long ticketId)
    {
        await _services.DeleteTicket(id, ticketId);
        return Ok();
    }
    [HttpDelete("{Id}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _services.Delete(id);
        return Ok();
    }
}