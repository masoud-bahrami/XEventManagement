using Quantum.ApplicationService;
using XEvent.CheckIn.Domain;
using XEvent.Reservation.Domain;

namespace XEvent.CheckIn.UseCaseHandlers;

public interface ICheckInService
{
    Task CheckInAsync(ReservationId reservationId, long attendeeId, CancellationToken ct);
    Task<IEnumerable<CheckInRecord>> GetAllByReservationAsync(ReservationId reservationId, CancellationToken ct);
}

public class CheckInService : ICheckInService
{
    private readonly ICheckInRepository _repository;
    private readonly IEventOutbox _outbox;

    public CheckInService(ICheckInRepository repository, IEventOutbox outbox)
    {
        _repository = repository;
        _outbox = outbox;
    }

    public async Task CheckInAsync(ReservationId reservationId, long attendeeId, CancellationToken ct)
    {
        var record = await (_repository as InMemoryCheckInRepository)?.GetAsync(reservationId, attendeeId, ct);

        if (record == null)
            throw new InvalidOperationException("Check-in record not found");

        if (record.Status == Domain.AttendeeStatus.CheckedIn)
            throw new InvalidOperationException("Attendee already checked in");

        record.MarkCheckedIn(DateTime.UtcNow);

        await _repository.SaveChangesAsync(ct);

        // Raise domain event
        await _outbox.Add(
            
            new[] {new AttendeeCheckedInEvent(reservationId, attendeeId, record.CheckedInAt)});
    }

    public Task<IEnumerable<CheckInRecord>> GetAllByReservationAsync(ReservationId reservationId, CancellationToken ct)
        => _repository.GetAllByReservationAsync(reservationId, ct);
}