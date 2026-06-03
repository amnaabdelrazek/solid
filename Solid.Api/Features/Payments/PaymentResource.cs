using Solid.Api.Common;

namespace Solid.Api.Features.Payments;

public static class PaymentResource
{
    public static object From(Dictionary<string, object?> payment)
    {
        return new
        {
            id = payment["id"],
            user_id = payment.GetValueOrDefault("user_id"),
            session_id = payment.GetValueOrDefault("session_id"),
            amount = payment.GetValueOrDefault("amount"),
            currency = payment.GetValueOrDefault("currency"),
            status = payment.GetValueOrDefault("status"),
            gateway = payment.GetValueOrDefault("gateway"),
            gateway_transaction_id = payment.GetValueOrDefault("gateway_transaction_id"),
            gateway_response = payment.JsonValue("gateway_response"),
            paid_at = payment.GetValueOrDefault("paid_at"),
            created_at = payment.GetValueOrDefault("created_at"),
            updated_at = payment.GetValueOrDefault("updated_at")
        };
    }
}
