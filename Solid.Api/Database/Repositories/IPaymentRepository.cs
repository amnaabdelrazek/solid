using Solid.Api.Database.Entities;

namespace Solid.Api.Database.Repositories;

public interface IPaymentRepository
{
    Task<Payment> CreatePendingAsync(long userId, long sessionId, decimal amount);

    Task<IReadOnlyList<Payment>> HistoryAsync(long userId);

    Task<PaymentMethod> CreatePaymentMethodAsync(long userId, PaymentMethodCreate create);

    Task<IReadOnlyList<PaymentMethod>> PaymentMethodsAsync(long userId);
}

public sealed record PaymentMethodCreate(string CardHolder, string CardNumber, string Expiry, bool IsDefault, string? GatewayToken);
