using XEvent.EventManagement.Domain.Contract.Commands;

public record CancelEventCommand(long EventId) : IsACommand;