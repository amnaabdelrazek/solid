using Microsoft.AspNetCore.Mvc;
using Solid.Api.Common;
using Solid.Api.Database.Repositories;
using Solid.Api.Features.Notifications;
using Solid.Api.Infrastructure.Auth;
using Stripe;
using Stripe.Checkout;

namespace Solid.Api.Features.Payments;

public static class PaymentSlice
{
    public static IEndpointRouteBuilder MapPaymentSlice(this IEndpointRouteBuilder api)
    {
        api.MapPost("/payments/initiate/{sessionId:long}", Initiate);
        api.MapGet("/payments/success", Success).AllowAnonymous();
        api.MapGet("/payments/cancel", Cancel).AllowAnonymous();
        api.MapGet("/payments/history", History);
        api.MapPost("/payment-methods", StorePaymentMethod)
            .Accepts<PaymentMethodRequest>("application/json", "application/x-www-form-urlencoded", "multipart/form-data");
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
        IConfiguration configuration,
        HttpRequest httpRequest)
    {
        var bookingValidation = await sessionRepository.ValidateBookingAsync(sessionId, auth.UserId);
        if (!bookingValidation.Success)
            return ApiResponse.Fail(bookingValidation.Error ?? "Unable to book session.", bookingValidation.StatusCode);

        var settingsAmount = await settingsRepository.GetAsync("general", "session_price");
        decimal.TryParse(settingsAmount, out var amount);

        var payment = await paymentRepository.CreatePendingAsync(auth.UserId, sessionId, amount);

        StripeConfiguration.ApiKey = configuration["Stripe:SecretKey"];

        var paymentMetadata = new Dictionary<string, string>
        {
            ["payment_id"] = payment.Id.ToString(),
            ["session_id"] = sessionId.ToString(),
            ["user_id"] = auth.UserId.ToString()
        };

        var options = new SessionCreateOptions
        {
            Metadata = paymentMetadata,
            PaymentIntentData = new SessionPaymentIntentDataOptions
            {
                Metadata = paymentMetadata
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
            SuccessUrl = $"{ResolveBaseUrl(configuration, httpRequest)}/api/payments/success?paymentId={payment.Id}&session_id={{CHECKOUT_SESSION_ID}}",
            CancelUrl = $"{ResolveBaseUrl(configuration, httpRequest)}/api/payments/cancel?paymentId={payment.Id}",
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

    private static async Task<IResult> Success(
        [FromQuery] long paymentId,
        [FromQuery(Name = "session_id")] string? checkoutSessionId,
        IPaymentRepository paymentRepository,
        IConfiguration configuration,
        INotificationService notificationService)
    {
        if (string.IsNullOrWhiteSpace(checkoutSessionId))
            return ApiResponse.Fail("Checkout session is required.", StatusCodes.Status422UnprocessableEntity);

        StripeConfiguration.ApiKey = configuration["Stripe:SecretKey"];

        var service = new SessionService();
        var checkoutSession = await service.GetAsync(checkoutSessionId);

        if (checkoutSession.PaymentStatus != "paid")
            return ApiResponse.Fail("Payment has not been completed.", StatusCodes.Status422UnprocessableEntity);

        var metadataPaymentId = checkoutSession.Metadata.TryGetValue("payment_id", out var checkoutPaymentId)
            ? checkoutPaymentId
            : null;

        if (metadataPaymentId is null && checkoutSession.PaymentIntentId is not null)
        {
            var paymentIntentService = new PaymentIntentService();
            var paymentIntent = await paymentIntentService.GetAsync(checkoutSession.PaymentIntentId);

            metadataPaymentId = paymentIntent.Metadata.TryGetValue("payment_id", out var intentPaymentId)
                ? intentPaymentId
                : null;
        }

        if (metadataPaymentId != paymentId.ToString())
        {
            return ApiResponse.Fail("Payment session mismatch.", StatusCodes.Status422UnprocessableEntity);
        }

        var paidPayment = await paymentRepository.MarkAsPaidAsync(paymentId, checkoutSession.Id);
        if (paidPayment is not null)
        {
            await NotifyPaymentPaidAsync(notificationService, paidPayment);
        }

        return ApiResponse.Ok(message: "Payment completed successfully.");
    }

    private static IResult Cancel([FromQuery] long paymentId)
    {
        return ApiResponse.Fail($"Payment #{paymentId} was cancelled.", StatusCodes.Status422UnprocessableEntity);
    }

    private static async Task<IResult> WebhookStripe(
        HttpRequest request,
        IPaymentRepository paymentRepository,
        IConfiguration configuration,
        INotificationService notificationService)
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
                    var paidPayment = await paymentRepository.MarkAsPaidAsync(paymentId, session.Id);
                    if (paidPayment is not null)
                    {
                        await NotifyPaymentPaidAsync(notificationService, paidPayment);
                    }
                }
            }
            else if (stripeEvent.Type == "payment_intent.succeeded")
            {
                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                if (paymentIntent?.Metadata.TryGetValue("payment_id", out var paymentIdStr) == true
                    && long.TryParse(paymentIdStr, out var paymentId))
                {
                    var paidPayment = await paymentRepository.MarkAsPaidAsync(paymentId, paymentIntent.Id);
                    if (paidPayment is not null)
                    {
                        await NotifyPaymentPaidAsync(notificationService, paidPayment);
                    }
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
        HttpRequest httpRequest,
        IPaymentRepository paymentRepository)
    {
        var payload = await RequestPayload.ReadAsync<PaymentMethodRequest>(httpRequest);
        if (payload.Error is not null)
            return payload.Error;

        var request = payload.Value!;

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

    private static string ResolveBaseUrl(IConfiguration configuration, HttpRequest request)
    {
        var configuredBaseUrl = configuration["App:BaseUrl"];

        if (!string.IsNullOrWhiteSpace(configuredBaseUrl) &&
            !configuredBaseUrl.Contains("your-domain.com", StringComparison.OrdinalIgnoreCase))
        {
            return configuredBaseUrl.TrimEnd('/');
        }

        return $"{request.Scheme}://{request.Host}".TrimEnd('/');
    }

    private static Task NotifyPaymentPaidAsync(INotificationService notificationService, Database.Entities.Payment payment)
    {
        return notificationService.NotifyUsersAsync(
            [payment.UserId],
            "SessionPaid",
            "Payment successful",
            $"Your session payment of {payment.Amount:0.##} {payment.Currency} was successful.",
            "credit-card",
            new
            {
                payment_id = payment.Id,
                session_id = payment.SessionId,
                amount = payment.Amount,
                currency = payment.Currency
            });
    }
}

public sealed record PaymentMethodRequest(string card_holder, string card_number, string expiry, bool is_default, string? gateway_token);
