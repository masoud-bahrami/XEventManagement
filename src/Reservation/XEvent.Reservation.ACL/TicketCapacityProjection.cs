using Quantum.ApplicationService;
using XEvent.Reservation.Domain;
using XEvent.Reservation.UseCaseHandlers;
using XEvent.Tickets.Domain.Contracts.Events;

namespace XEvent.Reservation.ACL;

public sealed class TicketCapacityProjection :
    IWantToProject<NewEventTicketIsAddedEvent>,
    IWantToProject<EventTicketIsUpdatedEvent>,
    IWantToProject<ATicketIsRemovedFromEvent>
{
    private readonly ITicketCapacityStore _capacityStore;

    public TicketCapacityProjection(
        ITicketCapacityStore capacityStore)
    {
        _capacityStore = capacityStore;
    }

    public async Task On(NewEventTicketIsAddedEvent @event,
        CancellationToken ct)
    {
        await _capacityStore.UpdateCapacityAsync(
            new EventId(@event.EventId),
            new TicketTypeId(@event.TicketId),
            @event.Ticket.Capacity,
            ct);
    }

    public async Task On(
        EventTicketIsUpdatedEvent @event,
        CancellationToken ct)
    {
        await _capacityStore.UpdateCapacityAsync(
            new EventId(@event.EventId),
            new TicketTypeId(@event.TicketId),
            @event.Ticket.Capacity,
            ct);
    }

    public async Task On(
        ATicketIsRemovedFromEvent @event,
        CancellationToken ct)
    {
        await _capacityStore.UpdateCapacityAsync(
            new EventId(@event.EventId),
            new TicketTypeId(@event.TicketId),
            0,
            ct);
    }
}