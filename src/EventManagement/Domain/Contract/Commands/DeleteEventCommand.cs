using XEvent.EventManagement.Domain.Contract.Commands;

public record DeleteEventCommand(long EventId) : IsACommand;