using Microsoft.EntityFrameworkCore;
using Solid.Api.Database.Entities;

namespace Solid.Api.Database.Repositories;

public sealed class PaymentRepository(SolidDbContext dbContext) : IPaymentRepository
{
    public async Task<Payment> CreatePendingAsync(long userId, long sessionId, decimal amount)
    {
        var now = DateTime.UtcNow;
        var payment = new Payment
        {
            UserId = userId,
            SessionId = sessionId,
            Amount = amount,
            Currency = "EGP",
            Status = "pending",
            Gateway = "manual",
            GatewayTransactionId = Guid.NewGuid().ToString(),
            CreatedAt = now,
            UpdatedAt = now
        };

        dbContext.Payments.Add(payment);
        await dbContext.SaveChangesAsync();

        return payment;
    }

    public async Task<IReadOnlyList<Payment>> HistoryAsync(long userId)
    {
        return await dbContext.Payments
            .AsNoTracking()
            .Where(payment => payment.UserId == userId)
            .OrderByDescending(payment => payment.Id)
            .Take(20)
            .ToListAsync();
    }

    public async Task<PaymentMethod> CreatePaymentMethodAsync(long userId, PaymentMethodCreate create)
    {
        var now = DateTime.UtcNow;
        var paymentMethod = new PaymentMethod
        {
            UserId = userId,
            CardHolder = create.CardHolder,
            CardNumber = create.CardNumber,
            Expiry = create.Expiry,
            IsDefault = create.IsDefault,
            GatewayToken = create.GatewayToken,
            CreatedAt = now,
            UpdatedAt = now
        };

        dbContext.PaymentMethods.Add(paymentMethod);
        await dbContext.SaveChangesAsync();

        return paymentMethod;
    }

    public async Task<IReadOnlyList<PaymentMethod>> PaymentMethodsAsync(long userId)
    {
        return await dbContext.PaymentMethods
            .AsNoTracking()
            .Where(paymentMethod => paymentMethod.UserId == userId)
            .OrderBy(paymentMethod => paymentMethod.Id)
            .ToListAsync();
    }
}
