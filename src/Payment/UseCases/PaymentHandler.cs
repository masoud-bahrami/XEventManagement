using Quantum.ApplicationService;
using XEvent.Payment.Domain;

public interface IPaymentService
{
    Task<PaymentId> CreatePayment(CreatePaymentCommand command, CancellationToken ct);
    Task ConfirmPayment(PaymentId paymentId, string token, CancellationToken ct);
    Task MarkPaymentSucceeded(PaymentId paymentId, CancellationToken ct);
    Task MarkPaymentFailed(PaymentId paymentId, CancellationToken ct);
}

public sealed class PaymentService : IPaymentService
{
    private readonly IAggregateStore<Payment, PaymentId> _store;
    private readonly IIdGenerator _idGenerator;
    private readonly IPaymentGateway _gateway;

    public PaymentService(
        IAggregateStore<Payment, PaymentId> store,
        IIdGenerator idGenerator,
        IPaymentGateway gateway)
    {
        _store = store;
        _idGenerator = idGenerator;
        _gateway = gateway;
    }

    public Task<PaymentId> CreatePayment(CreatePaymentCommand command, CancellationToken ct)
        => Task.FromResult(Create(command))
               .Pipe(Save);

    public Task ConfirmPayment(PaymentId paymentId, string token, CancellationToken ct)
        => ReConstitute(paymentId)
            .Pipe(p => ConfirmAsync(p, token, ct))
            .Pipe(Save);

    public Task MarkPaymentSucceeded(PaymentId paymentId, CancellationToken ct)
        => ReConstitute(paymentId)
            .Pipe(p => { p.MarkSucceeded(); return Task.CompletedTask; })
            .Pipe(Save);

    public Task MarkPaymentFailed(PaymentId paymentId, CancellationToken ct)
        => ReConstitute(paymentId)
            .Pipe(p => { p.MarkFailed(); return Task.CompletedTask; })
            .Pipe(Save);

    private Payment Create(CreatePaymentCommand command)
    {
        var paymentId = new PaymentId(_idGenerator.NextId());
        var payment = new Payment(paymentId, command.ReservationId, command.Amount);

        // Optional: trigger the payment gateway request immediately
        // or defer to ConfirmPayment
        // var token = await _gateway.RequestPaymentAsync(paymentId, command.Amount, ct);

        return payment;
    }

    private async Task<Payment> ConfirmAsync(Payment payment, string token, CancellationToken ct)
    {
        var result = await _gateway.ConfirmPaymentAsync(token, ct);

        if (result.Success)
            payment.MarkSucceeded();
        else
            payment.MarkFailed();

        return payment;
    }

    private async Task<Payment> ReConstitute(PaymentId paymentId)
        => _store.Get(paymentId)
           ?? throw new KeyNotFoundException("Payment not found");

    private Task<PaymentId> Save(Payment payment)
    {
        _store.Save(payment);
        return Task.FromResult(payment.Id);
    }
}
