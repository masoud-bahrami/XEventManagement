using Quantum.Domain;

namespace XEvent.Tickets.Domain;

public record  TicketId : IsAnAggregateRootId
{

    public TicketId(long value) : base(value)
    {
    }
}