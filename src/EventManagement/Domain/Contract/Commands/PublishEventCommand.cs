using XEvent.EventManagement.Domain.Contract.Commands;

public record PublishEventCommand(long EventId) : IsACommand;