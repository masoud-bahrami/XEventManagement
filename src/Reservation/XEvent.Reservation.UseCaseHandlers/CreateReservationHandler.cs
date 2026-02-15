using Quantum.ApplicationService;
using XEvent.Reservation.Domain;

namespace XEvent.Reservation.UseCaseHandlers;

public interface IReservationServices
{
    Task<ReservationId> Reserve(CreateReservationCommand command, CancellationToken ct);
    Task Confirm(ReservationId reservationId, CancellationToken ct);
    Task Cancel(ReservationId reservationId, CancellationToken ct);
}

public sealed class ReservationService : IReservationServices
{
    private readonly ITicketCapacityStore _capacityStore;
    private readonly IReservationRepository _reservationRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IIdGenerator _idGenerator;

    private static readonly TimeSpan ReservationTtl = TimeSpan.FromMinutes(10);

    public ReservationService(
        ITicketCapacityStore capacityStore,
        IReservationRepository reservationRepository,
        ICurrentUser currentUser,
        IIdGenerator idGenerator)
    {
        _capacityStore = capacityStore;
        _reservationRepository = reservationRepository;
        _currentUser = currentUser;
        _idGenerator = idGenerator;
    }

    public Task<ReservationId> Reserve(CreateReservationCommand command, CancellationToken ct)
        => Task.FromResult(Create(command))
               .Pipe(Save);

    public Task Confirm(ReservationId reservationId, CancellationToken ct)
        => ReConstitute(reservationId)
            .Pipe(EnsureOwnership)
            .Pipe(r => { r.MarkConfirmed(); return Task.CompletedTask; })
            .Pipe(Save);

    public Task Cancel(ReservationId reservationId, CancellationToken ct)
        => ReConstitute(reservationId)
            .Pipe(EnsureOwnership)
            .Pipe(r => { r.MarkCancelled(); return Task.CompletedTask; })
            .Pipe(Save);

    private Domain.Reservation Create(CreateReservationCommand command)
    {
        var reservationId = new ReservationId(_idGenerator.NextId());
        var buyerId = new BuyerId(_currentUser.UserId);

        var reserved = _capacityStore.TryReserveAsync(
            reservationId,
            new EventId(command.EventId),
            new TicketTypeId(command.TicketTypeId),
            command.Attendees.Count,
            ReservationTtl,
            CancellationToken.None).GetAwaiter().GetResult();

        if (!reserved)
            throw new ApplicationException("Capacity exceeded");

        return new Domain.Reservation(reservationId, buyerId, command, ReservationTtl);
    }

    private async Task<Domain.Reservation> ReConstitute(ReservationId reservationId)
        => await _reservationRepository.GetAsync(reservationId, CancellationToken.None)
           ?? throw new KeyNotFoundException("Reservation not found.");

    private Task<Domain.Reservation> EnsureOwnership(Domain.Reservation reservation)
    {
        if (reservation.BuyerId.Value != _currentUser.UserId)
            throw new UnauthorizedAccessException("User is not the owner of this reservation.");

        return Task.FromResult(reservation);
    }

    private async Task<ReservationId> Save(Domain.Reservation reservation)
    {
        await _reservationRepository.UpdateAsync(reservation, CancellationToken.None);
        return reservation.Id;
    }
}