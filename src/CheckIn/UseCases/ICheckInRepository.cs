using System.Collections.Concurrent;
using XEvent.CheckIn.Domain;
using XEvent.Reservation.Domain;

namespace XEvent.CheckIn.UseCaseHandlers;

public interface ICheckInRepository
{
    Task AddAsync(CheckInRecord record, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
    Task<IEnumerable<CheckInRecord>> GetAllByReservationAsync(ReservationId reservationId, CancellationToken ct);
}

public class InMemoryCheckInRepository : ICheckInRepository
{
    private readonly ConcurrentDictionary<(ReservationId, long), CheckInRecord> _store
        = new();

    public Task AddAsync(CheckInRecord record, CancellationToken ct)
    {
        _store.TryAdd((record.ReservationId, record.AttendeeId), record);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken ct)
    {
        // In-memory: nothing to commit
        return Task.CompletedTask;
    }

    // Additional helpers
    public Task<CheckInRecord?> GetAsync(ReservationId reservationId, long attendeeId, CancellationToken ct)
    {
        _store.TryGetValue((reservationId, attendeeId), out var record);
        return Task.FromResult(record);
    }

    public Task<IEnumerable<CheckInRecord>> GetAllByReservationAsync(ReservationId reservationId, CancellationToken ct)
    {
        var records = _store.Values.Where(r => r.ReservationId == reservationId);
        return Task.FromResult(records);
    }
}
