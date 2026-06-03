using Microsoft.AspNetCore.Mvc;
using Solid.Api.Common;
using Solid.Api.Features.Shared;
using Solid.Api.Infrastructure.Auth;
using Solid.Api.Database;

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

    private static async Task<IResult> Initiate(long sessionId, IAuthContext auth, IDatabase database)
    {
        var settingsAmount = await database.SettingAsync("general", "session_price");
        decimal.TryParse(settingsAmount, out var amount);
        var transactionId = Guid.NewGuid().ToString();
        var paymentId = await database.ExecuteScalarAsync<long>(
            """
            INSERT INTO payments (user_id, session_id, amount, currency, [status], gateway, gateway_transaction_id, created_at, updated_at)
            OUTPUT INSERTED.id
            VALUES (@userId, @sessionId, @amount, 'EGP', 'pending', 'manual', @transactionId, SYSDATETIME(), SYSDATETIME())
            """,
            new { auth.UserId, sessionId, amount, transactionId });
        var payment = (await database.QuerySingleAsync("SELECT TOP 1 * FROM payments WHERE id = @paymentId", new { paymentId }))!;

        return ApiResponse.Ok(new { payment = PaymentResource.From(payment), payment_url = "" });
    }

    private static async Task<IResult> History(IAuthContext auth, IDatabase database)
    {
        var payments = await database.QueryAsync("SELECT * FROM payments WHERE user_id = @userId ORDER BY id DESC OFFSET 0 ROWS FETCH NEXT 20 ROWS ONLY", new { auth.UserId });

        return ApiResponse.Ok(payments.Select(PaymentResource.From));
    }

    private static async Task<IResult> StorePaymentMethod(IAuthContext auth, [FromBody] PaymentMethodRequest request, IDatabase database)
    {
        var paymentMethodId = await database.ExecuteScalarAsync<long>(
            """
            INSERT INTO payment_methods (user_id, card_type, card_number, expiry, is_default, gateway_token, created_at, updated_at)
            OUTPUT INSERTED.id
            VALUES (@userId, @card_holder, @card_number, @expiry, @is_default, @gateway_token, SYSDATETIME(), SYSDATETIME())
            """,
            new { auth.UserId, request.card_holder, request.card_number, request.expiry, request.is_default, request.gateway_token });
        var paymentMethod = (await database.QuerySingleAsync("SELECT TOP 1 * FROM payment_methods WHERE id = @paymentMethodId", new { paymentMethodId }))!;

        return ApiResponse.Ok(new { payment_method = PaymentMethodResource.From(paymentMethod) }, "Payment method added successfully.");
    }

    private static async Task<IResult> PaymentMethods(IAuthContext auth, IDatabase database)
    {
        var paymentMethods = await database.QueryAsync("SELECT * FROM payment_methods WHERE user_id = @userId ORDER BY id", new { auth.UserId });

        return ApiResponse.Ok(new { payment_methods = paymentMethods.Select(PaymentMethodResource.From) });
    }
}

public sealed record PaymentMethodRequest(string card_holder, string card_number, string expiry, bool is_default, string? gateway_token);
