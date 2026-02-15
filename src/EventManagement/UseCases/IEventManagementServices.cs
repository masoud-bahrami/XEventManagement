using Quantum.ApplicationService;
using XEvent.EventManagement.Domain;
using XEvent.EventManagement.Domain.Contract.Commands;

namespace XEvent.EventManagement.UseCaseHandlers;

public interface IEventManagementServices
{
    Task Handle(CreateNewEventCommand command);
    Task Handle(long id, UpdateEventCommand updateEventCommand);
    Task Handle(long id, long ticketId, UpdateTicketsCommand cmd);
    Task Delete(long id);
    Task Handle(long id, AddNewTicketCommand cmd);
    Task DeleteTicket(long id, long ticketId);
}
public sealed class EventManagementService : IEventManagementServices
{
    private readonly IEventRepository _repository;
    private readonly IEventOutbox _outbox;
    private readonly ICurrentUser _currentUser;
    private readonly IIdGenerator _idGenerator;
    private readonly IUnitOfWork _uow;

    public EventManagementService(
        IEventRepository repository,
        IEventOutbox outbox,
        ICurrentUser currentUser,
        IIdGenerator idGenerator,
        IUnitOfWork uow)
    {
        _repository = repository;
        _outbox = outbox;
        _currentUser = currentUser;
        _idGenerator = idGenerator;
        _uow = uow;
    }

    public Task Handle(CreateNewEventCommand command)
        => Task.FromResult(Create(command))
               .Pipe(Save);

    public Task Handle(long id, UpdateEventCommand command)
        => ReConstitute(id)
           .Pipe(EnsureOwnership)
           .Pipe(evt => { evt.Handle(command); return Task.CompletedTask; })
           .Pipe(Save);

    public Task Handle(long id,long ticketId,  UpdateTicketsCommand cmd)
        => ReConstitute(id)
            .Pipe(EnsureOwnership)
            .Pipe(evt => { evt.Handle(ticketId, cmd); return Task.CompletedTask; })
            .Pipe(Save);


    public Task Delete(long id)
        => ReConstitute(id)
           .Pipe(EnsureOwnership)
           .Pipe(evt => { evt.Handle(new DeleteEventCommand(id)); return Task.CompletedTask; })
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


    private Event Create(CreateNewEventCommand command)
        => new Event(_idGenerator.NextId(), _currentUser.UserId, command);

    private async Task<Event> ReConstitute(long id)
        => await _repository.Load(new EventId(id));

    private async Task<Event> EnsureOwnership(Event evt)
    {
        if (evt.Owner != _currentUser.UserId)
            throw new UnauthorizedAccessException("User is not the owner of this event.");
        
        return evt;
    }

    private async Task<Event> Save(Event evt)
    {
        await _repository.Save(evt);
        await _outbox.Add(evt.GetUncommittedEvents());
        await _uow.Commit();
        return evt;
    }
}