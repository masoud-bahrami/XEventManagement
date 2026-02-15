namespace XEvent.Tickets.Domain.UseCaseHandlers;

public interface ITicketsRepository
{
    Task Save(Tickets aggregate);
    Task<Tickets> Load(TicketId id);
}