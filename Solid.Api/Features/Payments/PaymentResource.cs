using Solid.Api.Common;
using Solid.Api.Database.Entities;

namespace Solid.Api.Features.Payments;

public static class PaymentResource
{
    public static object From(Payment payment)
    {
        return new
        {
            id = payment.Id,
            user_id = payment.UserId,
            session_id = payment.SessionId,
            amount = payment.Amount,
            currency = payment.Currency,
            status = payment.Status,
            gateway = payment.Gateway,
            gateway_transaction_id = payment.GatewayTransactionId,
            gateway_response = JsonPayload.Parse(payment.GatewayResponse),
            paid_at = EgyptDateTime.Format(payment.PaidAt),
            created_at = EgyptDateTime.Format(payment.CreatedAt),
            updated_at = EgyptDateTime.Format(payment.UpdatedAt)
        };
    }
}
