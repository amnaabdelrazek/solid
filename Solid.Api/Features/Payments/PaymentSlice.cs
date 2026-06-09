using Microsoft.AspNetCore.Mvc;
using Solid.Api.Common;
using Solid.Api.Database.Repositories;
using Solid.Api.Infrastructure.Auth;
using Stripe;
using Stripe.Checkout;

namespace Solid.Api.Features.Payments;

public static class PaymentSlice
{
    public static IEndpointRouteBuilder MapPaymentSlice(this IEndpointRouteBuilder api)
    {
        api.MapPost("/payments/initiate/{sessionId:long}", Initiate);
        api.MapGet("/payments/history", History);
        api.MapPost("/payment-methods", StorePaymentMethod);
        api.MapGet("/payment-methods", PaymentMethods);
        api.MapPost("/payments/webhook/stripe", WebhookStripe).AllowAnonymous();

        return api;
    }

    private static async Task<IResult> Initiate(
        long sessionId,
        IAuthContext auth,
        ISettingsRepository settingsRepository,
        IPaymentRepository paymentRepository,
        ISessionRepository sessionRepository,
        IConfiguration configuration)
    {
        if (await sessionRepository.FindAnyAsync(sessionId) is null)
            return ApiResponse.Fail("Session not found.", StatusCodes.Status404NotFound);

        var settingsAmount = await settingsRepository.GetAsync("general", "session_price");
        decimal.TryParse(settingsAmount, out var amount);

        var payment = await paymentRepository.CreatePendingAsync(auth.UserId, sessionId, amount);

        StripeConfiguration.ApiKey = configuration["Stripe:SecretKey"];

        var options = new SessionCreateOptions
        {
            PaymentIntentData = new SessionPaymentIntentDataOptions
            {
                Metadata = new Dictionary<string, string>
                {
                    ["payment_id"] = payment.Id.ToString(),
                    ["session_id"] = sessionId.ToString(),
                    ["user_id"] = auth.UserId.ToString()
                }
            },
            PaymentMethodTypes = ["card"],
            LineItems =
            [
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "egp",
                        UnitAmount = (long)(amount * 100),
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = $"Therapy Session #{sessionId}",
                        }
                    },
                    Quantity = 1,
                }
            ],
            Mode = "payment",
            SuccessUrl = $"{configuration["App:BaseUrl"]}/api/payments/success?paymentId={payment.Id}&session_id={{CHECKOUT_SESSION_ID}}",
            CancelUrl = $"{configuration["App:BaseUrl"]}/api/payments/cancel?paymentId={payment.Id}",
        };

        var service = new SessionService();
        var checkoutSession = await service.CreateAsync(options);

        await paymentRepository.UpdateGatewayTransactionAsync(payment.Id, checkoutSession.Id, "stripe");

        return ApiResponse.Ok(new
        {
            payment = PaymentResource.From(payment),
            payment_url = checkoutSession.Url
        });
    }

    private static async Task<IResult> WebhookStripe(
        HttpRequest request,
        IPaymentRepository paymentRepository,
        IConfiguration configuration)
    {
        var json = await new StreamReader(request.Body).ReadToEndAsync();

        // Stripe webhook secret is required in production
        var webhookSecret = configuration["Stripe:WebhookSecret"];
        if (string.IsNullOrWhiteSpace(webhookSecret))
            return Results.BadRequest();

        try
        {
            var stripeEvent = EventUtility.ConstructEvent(
                json,
                request.Headers["Stripe-Signature"],
                webhookSecret
            );

            if (stripeEvent.Type == "checkout.session.completed")
            {
                var session = stripeEvent.Data.Object as Stripe.Checkout.Session;
                if (session?.Metadata.TryGetValue("payment_id", out var paymentIdStr) == true
                    && long.TryParse(paymentIdStr, out var paymentId))
                {
                    var payments = await paymentRepository.HistoryAsync(0); // just used to check existence
                    // Mark as paid only if payment exists
                    await paymentRepository.MarkAsPaidAsync(paymentId, session.Id);
                }
            }
            else if (stripeEvent.Type == "payment_intent.succeeded")
            {
                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                if (paymentIntent?.Metadata.TryGetValue("payment_id", out var paymentIdStr) == true
                    && long.TryParse(paymentIdStr, out var paymentId))
                {
                    await paymentRepository.MarkAsPaidAsync(paymentId, paymentIntent.Id);
                }
            }

            return Results.Ok();
        }
        catch (StripeException)
        {
            return Results.BadRequest();
        }
    }

    private static async Task<IResult> History(IAuthContext auth, IPaymentRepository paymentRepository)
    {
        var payments = await paymentRepository.HistoryAsync(auth.UserId);
        return ApiResponse.Ok(payments.Select(PaymentResource.From));
    }

    private static async Task<IResult> StorePaymentMethod(
        IAuthContext auth,
        [FromBody] PaymentMethodRequest request,
        IPaymentRepository paymentRepository)
    {
        if (string.IsNullOrWhiteSpace(request.card_holder))
            return ApiResponse.Fail("card_holder is required.", StatusCodes.Status422UnprocessableEntity);

        if (string.IsNullOrWhiteSpace(request.card_number))
            return ApiResponse.Fail("card_number is required.", StatusCodes.Status422UnprocessableEntity);

        if (string.IsNullOrWhiteSpace(request.expiry))
            return ApiResponse.Fail("expiry is required.", StatusCodes.Status422UnprocessableEntity);

        var paymentMethod = await paymentRepository.CreatePaymentMethodAsync(
            auth.UserId,
            new PaymentMethodCreate(request.card_holder, request.card_number, request.expiry, request.is_default, request.gateway_token));

        return ApiResponse.Ok(new { payment_method = PaymentMethodResource.From(paymentMethod) }, "Payment method added successfully.");
    }

    private static async Task<IResult> PaymentMethods(IAuthContext auth, IPaymentRepository paymentRepository)
    {
        var paymentMethods = await paymentRepository.PaymentMethodsAsync(auth.UserId);
        return ApiResponse.Ok(new { payment_methods = paymentMethods.Select(PaymentMethodResource.From) });
    }
}

public sealed record PaymentMethodRequest(string card_holder, string card_number, string expiry, bool is_default, string? gateway_token);
