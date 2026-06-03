namespace Solid.Api.Database.Repositories;

public sealed class PaymentRepository(IDatabase database) : IPaymentRepository
{
    public async Task<Dictionary<string, object?>> CreatePendingAsync(long userId, long sessionId, decimal amount)
    {
        var transactionId = Guid.NewGuid().ToString();
        var paymentId = await database.ExecuteScalarAsync<long>(
            """
            INSERT INTO payments (user_id, session_id, amount, currency, [status], gateway, gateway_transaction_id, created_at, updated_at)
            OUTPUT INSERTED.id
            VALUES (@userId, @sessionId, @amount, 'EGP', 'pending', 'manual', @transactionId, SYSDATETIME(), SYSDATETIME())
            """,
            new { userId, sessionId, amount, transactionId });

        return (await database.QuerySingleAsync("SELECT TOP 1 * FROM payments WHERE id = @paymentId", new { paymentId }))!;
    }

    public async Task<IReadOnlyList<Dictionary<string, object?>>> HistoryAsync(long userId)
    {
        return await database.QueryAsync("SELECT * FROM payments WHERE user_id = @userId ORDER BY id DESC OFFSET 0 ROWS FETCH NEXT 20 ROWS ONLY", new { userId });
    }

    public async Task<Dictionary<string, object?>> CreatePaymentMethodAsync(long userId, PaymentMethodCreate create)
    {
        var paymentMethodId = await database.ExecuteScalarAsync<long>(
            """
            INSERT INTO payment_methods (user_id, card_type, card_number, expiry, is_default, gateway_token, created_at, updated_at)
            OUTPUT INSERTED.id
            VALUES (@userId, @CardHolder, @CardNumber, @Expiry, @IsDefault, @GatewayToken, SYSDATETIME(), SYSDATETIME())
            """,
            new { userId, create.CardHolder, create.CardNumber, create.Expiry, create.IsDefault, create.GatewayToken });

        return (await database.QuerySingleAsync("SELECT TOP 1 * FROM payment_methods WHERE id = @paymentMethodId", new { paymentMethodId }))!;
    }

    public async Task<IReadOnlyList<Dictionary<string, object?>>> PaymentMethodsAsync(long userId)
    {
        return await database.QueryAsync("SELECT * FROM payment_methods WHERE user_id = @userId ORDER BY id", new { userId });
    }
}
