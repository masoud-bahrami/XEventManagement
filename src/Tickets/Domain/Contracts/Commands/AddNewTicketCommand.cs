using Quantum.Domain;
using XEvent.Tickets.Domain.Contracts.Events;

namespace XEvent.Tickets.Domain.Contracts.Commands;

public record AddNewTicketCommand(TicketDto Ticket) : IsACommand;