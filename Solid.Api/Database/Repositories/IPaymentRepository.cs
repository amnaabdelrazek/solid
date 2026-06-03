namespace Solid.Api.Database.Repositories;

public interface IPaymentRepository
{
    Task<Dictionary<string, object?>> CreatePendingAsync(long userId, long sessionId, decimal amount);

    Task<IReadOnlyList<Dictionary<string, object?>>> HistoryAsync(long userId);

    Task<Dictionary<string, object?>> CreatePaymentMethodAsync(long userId, PaymentMethodCreate create);

    Task<IReadOnlyList<Dictionary<string, object?>>> PaymentMethodsAsync(long userId);
}

public sealed record PaymentMethodCreate(string CardHolder, string CardNumber, string Expiry, bool IsDefault, string? GatewayToken);
