using Microsoft.AspNetCore.Mvc;
using XEvent.Payment.Domain;
using XEvent.Payment.UseCases;

[ApiController]
[Route("api/payments")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentCommand command)
    {
        var paymentId = await _paymentService.CreatePayment(command, CancellationToken.None);
        return Ok(paymentId);
    }

    [HttpPost("{id}/succeed")]
    public async Task<IActionResult> MarkSucceeded(long id)
    {
        await _paymentService.MarkPaymentSucceeded(new PaymentId(id), CancellationToken.None);
        return Ok();
    }

    [HttpPost("{id}/fail")]
    public async Task<IActionResult> MarkFailed(long id)
    {
        await _paymentService.MarkPaymentFailed(new PaymentId(id), CancellationToken.None);
        return Ok();
    }
}