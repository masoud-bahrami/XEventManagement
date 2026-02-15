using Quantum.ApplicationService;
using XEvent.Tickets.Domain.Contracts.Commands;

namespace XEvent.Tickets.Domain.UseCaseHandlers;

public interface ITicketServices
{
    Task Handle(long id, long ticketId, UpdateTicketsCommand cmd);
    Task Handle(long id, AddNewTicketCommand cmd);
    Task DeleteTicket(long id, long ticketId);
}
public sealed class TicketService : ITicketServices
{
    private readonly ITicketsRepository _repository;
    private readonly IEventOutbox _outbox;
    private readonly IUnitOfWork _uow;

    public TicketService(
        ITicketsRepository repository,
        IEventOutbox outbox,
        IUnitOfWork uow)
    {
        _repository = repository;
        _outbox = outbox;
        _uow = uow;
    }
    
    public Task Handle(long id,long ticketId,  UpdateTicketsCommand cmd)
        => ReConstitute(id)
            .Pipe(EnsureOwnership)
            .Pipe(evt => { evt.Handle(ticketId, cmd); return Task.CompletedTask; })
            .Pipe(Save);

    
    public Task Handle(long id, AddNewTicketCommand cmd)
                => ReConstitute(id)
                .Pipe(EnsureOwnership)
                .Pipe(evt => { evt.Handle(cmd); return Task.CompletedTask; })
                .Pipe(Save);

    public Task DeleteTicket(long id, long ticketId)
        => ReConstitute(id)
            .Pipe(EnsureOwnership)
            .Pipe(evt => { evt.Handle(new DeleteTicketCommand(ticketId)); return Task.CompletedTask; })
            .Pipe(Save);

    
    private async Task<Tickets> ReConstitute(long id)
        => await _repository.Load(new TicketId(id));

    private async Task<Tickets> EnsureOwnership(Tickets evt)
    {
        //if (evt.Owner != _currentUser.UserId)
        //    throw new UnauthorizedAccessException("User is not the owner of this event.");
        
        return evt;
    }

    private async Task<Tickets> Save(Tickets evt)
    {
        await _repository.Save(evt);
        await _outbox.Add(evt.GetUncommittedEvents());
        await _uow.Commit();
        return evt;
    }
}