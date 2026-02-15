using XEvent.Payment.Domain;

public interface IPaymentGateway
{
    Task<string> RequestPaymentAsync(PaymentId paymentId, decimal amount, CancellationToken ct);
    
    Task<PaymentResult> ConfirmPaymentAsync(string paymentToken, CancellationToken ct);
}

public class PaymentResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}

public class FakePaymentGateway : IPaymentGateway
{
    private readonly Dictionary<string, PaymentResult> _payments = new();

    public Task<string> RequestPaymentAsync(PaymentId paymentId, decimal amount, CancellationToken ct)
    {
        var token = Guid.NewGuid().ToString();
        _payments[token] = new PaymentResult
        {
            Success = false 
        };
        return Task.FromResult(token);
    }

    public Task<PaymentResult> ConfirmPaymentAsync(string paymentToken, CancellationToken ct)
    {
        if (_payments.ContainsKey(paymentToken))
        {
            var success = new Random().NextDouble() < 0.8;
            _payments[paymentToken] = new PaymentResult
            {
                Success = success,
                ErrorMessage = success ? null : "Payment failed"
            };
            return Task.FromResult(_payments[paymentToken]);
        }

        return Task.FromResult(new PaymentResult
        {
            Success = false,
            ErrorMessage = "Invalid token"
        });
    }
}
