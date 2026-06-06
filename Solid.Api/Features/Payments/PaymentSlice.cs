using Microsoft.AspNetCore.Mvc;
using Solid.Api.Common;
using Solid.Api.Database.Repositories;
using Solid.Api.Infrastructure.Auth;

namespace Solid.Api.Features.Payments;

public static class PaymentSlice
{
    public static IEndpointRouteBuilder MapPaymentSlice(this IEndpointRouteBuilder api)
    {
        api.MapPost("/payments/initiate/{sessionId:long}", Initiate);
        api.MapGet("/payments/history", History);
        api.MapPost("/payment-methods", StorePaymentMethod);
        api.MapGet("/payment-methods", PaymentMethods);

        return api;
    }

    private static async Task<IResult> Initiate(
        long sessionId,
        IAuthContext auth,
        ISettingsRepository settingsRepository,
        IPaymentRepository paymentRepository,
        ISessionRepository sessionRepository)
    {
        if (await sessionRepository.FindAnyAsync(sessionId) is null)
        {
            return ApiResponse.Fail("Session not found.", StatusCodes.Status404NotFound);
        }

        var settingsAmount = await settingsRepository.GetAsync("general", "session_price");
        decimal.TryParse(settingsAmount, out var amount);
        var payment = await paymentRepository.CreatePendingAsync(auth.UserId, sessionId, amount);

        return ApiResponse.Ok(new { payment = PaymentResource.From(payment), payment_url = "" });
    }

    private static async Task<IResult> History(IAuthContext auth, IPaymentRepository paymentRepository)
    {
        var payments = await paymentRepository.HistoryAsync(auth.UserId);

        return ApiResponse.Ok(payments.Select(PaymentResource.From));
    }

    private static async Task<IResult> StorePaymentMethod(IAuthContext auth, [FromBody] PaymentMethodRequest request, IPaymentRepository paymentRepository)
    {
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
