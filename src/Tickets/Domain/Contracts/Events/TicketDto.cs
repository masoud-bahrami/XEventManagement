namespace XEvent.Tickets.Domain.Contracts.Events;

public record TicketDto(string Name, decimal Price, short Capacity);
