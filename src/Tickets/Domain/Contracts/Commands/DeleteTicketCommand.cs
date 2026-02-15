using Quantum.Domain;

namespace XEvent.Tickets.Domain.Contracts.Commands;

public record DeleteTicketCommand(long TicketId) : IsACommand;