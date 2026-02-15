using XEvent.Payment.Domain;

namespace XEvent.Payment.UseCases;

public interface IPaymentService
{
    Task<PaymentId> CreatePaymentAsync(CreatePaymentCommand command, CancellationToken ct);
    Task ConfirmPaymentAsync(PaymentId paymentId, string token, CancellationToken ct);
    Task MarkPaymentSucceededAsync(PaymentId paymentId, CancellationToken ct);
    Task MarkPaymentFailedAsync(PaymentId paymentId, CancellationToken ct);
}