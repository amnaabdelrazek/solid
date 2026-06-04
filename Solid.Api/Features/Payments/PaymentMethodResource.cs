using Solid.Api.Database.Entities;

namespace Solid.Api.Features.Payments;

public static class PaymentMethodResource
{
    public static object From(PaymentMethod method)
    {
        return new
        {
            id = method.Id,
            card_holder = method.CardHolder ?? method.CardType,
            card_number = method.CardNumber,
            expiry = method.Expiry,
            is_default = method.IsDefault
        };
    }
}
