using Microsoft.AspNetCore.Mvc;
using MiHairCareApp.Application.DTO;
using MiHairCareApp.Application.Interfaces.Services;
using Stripe;


[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly ILogger<PaymentController> _logger;
    private readonly IPaymentService _paymentService;
    private readonly string _stripeSecret;

    public PaymentController(ILogger<PaymentController> logger, IConfiguration configuration, IPaymentService paymentService)
    {
        _logger = logger;
        _paymentService = paymentService;
        _stripeSecret = configuration["Stripe:SecretKey"]!;
        StripeConfiguration.ApiKey = _stripeSecret;
    }

    [HttpPost("create-intent")]
    public async Task<IActionResult> CreateIntent([FromBody] CreatePaymentRequestDto req)
    {
        if (req == null || req.Amount <= 0)
            return BadRequest("Invalid payment request.");

        var result = await _paymentService.CreatePaymentIntentAsync(req);

        return StatusCode(result.StatusCode, result);
    }


    [HttpPost("webhook")]
    public async Task<IActionResult> StripeWebhook()
    {
        try
        {
            await _paymentService.HandleStripeWebhookAsync(Request);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}


