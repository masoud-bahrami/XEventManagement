using XEvent.EventManagement.Domain.Contract.Commands;

public record MoveEventToDraftCommand(long EventId) : IsACommand;