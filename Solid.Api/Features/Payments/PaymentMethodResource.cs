namespace Solid.Api.Features.Payments;

public static class PaymentMethodResource
{
    public static object From(Dictionary<string, object?> method)
    {
        return new
        {
            id = method["id"],
            card_holder = method.GetValueOrDefault("card_type"),
            card_number = method.GetValueOrDefault("card_number"),
            expiry = method.GetValueOrDefault("expiry"),
            is_default = method.GetValueOrDefault("is_default")
        };
    }
}
