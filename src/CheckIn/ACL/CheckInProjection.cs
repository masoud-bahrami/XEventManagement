using Quantum.ApplicationService;
using XEvent.CheckIn.Domain;
using XEvent.CheckIn.UseCaseHandlers;
using XEvent.Reservation.UseCaseHandlers;

namespace XEvent.CheckIn.ACL;

public sealed class CheckInProjection :
    IWantToProject<Reservation.Domain.Reservation.ReservationConfirmedEvent>
{
    private readonly ICheckInRepository _repository;
    private readonly IReservationRepository _reservationRepository;

    public CheckInProjection(ICheckInRepository repository, IReservationRepository reservationRepository)
    {
        _repository = repository;
        _reservationRepository = reservationRepository;
    }

    public async Task On(Reservation.Domain.Reservation.ReservationConfirmedEvent @event, CancellationToken ct)
    {
        var reservation = await _reservationRepository.GetAsync(@event.ReservationId, ct);

        var checkIns = reservation.Attendees.Select(attendee =>
            new CheckInRecord(
                reservation.Id,         
                attendee.Id,                       
                DateTime.MinValue       
            )
        ).ToList();
        
        foreach (var ci in checkIns)
            await _repository.AddAsync(ci, ct);

        await _repository.SaveChangesAsync(ct);
    }
}