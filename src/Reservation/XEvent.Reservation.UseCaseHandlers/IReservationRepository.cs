using System.Collections.Concurrent;
using XEvent.Reservation.Domain;

namespace XEvent.Reservation.UseCaseHandlers;

public interface IReservationRepository
{
    Task AddAsync(Domain.Reservation reservation, CancellationToken ct);
    Task<Domain.Reservation?> GetAsync(ReservationId id, CancellationToken ct);
    Task UpdateAsync(Domain.Reservation reservation, CancellationToken ct);
}


public sealed class InMemoryReservationRepository : IReservationRepository
{
    private readonly ConcurrentDictionary<ReservationId, Domain.Reservation> _store = new();

    public Task AddAsync(Domain.Reservation reservation, CancellationToken ct)
    {
        if (!_store.TryAdd(reservation.Id, reservation))
            throw new InvalidOperationException($"Reservation {reservation.Id} already exists");

        return Task.CompletedTask;
    }

    public Task<Domain.Reservation?> GetAsync(ReservationId id, CancellationToken ct)
    {
        _store.TryGetValue(id, out var reservation);
        return Task.FromResult(reservation);
    }

    public Task UpdateAsync(Domain.Reservation reservation, CancellationToken ct)
    {
        _store.AddOrUpdate(
            reservation.Id,
            reservation,
            (_, existing) =>
            {
                //if (reservation.Version <= existing.Version)
                //    throw new InvalidOperationException("Concurrency conflict");
                return reservation;
            });
        return Task.CompletedTask;
    }
}
