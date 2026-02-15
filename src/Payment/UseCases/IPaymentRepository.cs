using System.Collections.Concurrent;
using XEvent.Payment.Domain;

public interface IPaymentRepository
{
    Task AddAsync(Payment payment, CancellationToken ct);
    Task<Payment?> GetAsync(PaymentId id, CancellationToken ct);
    Task UpdateAsync(Payment payment, CancellationToken ct);
}

public class InMemoryPaymentRepository : IPaymentRepository
{
    private readonly ConcurrentDictionary<long, Payment> _store = new();

    public Task AddAsync(Payment payment, CancellationToken ct)
    {
        _store.TryAdd(payment.Id.Value, payment);
        return Task.CompletedTask;
    }

    public Task<Payment?> GetAsync(PaymentId id, CancellationToken ct)
    {
        _store.TryGetValue(id.Value, out var payment);
        return Task.FromResult(payment);
    }

    public Task UpdateAsync(Payment payment, CancellationToken ct)
    {
        _store[payment.Id.Value] = payment;
        return Task.CompletedTask;
    }
}