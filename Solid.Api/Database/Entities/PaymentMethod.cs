namespace Solid.Api.Database.Entities;

public sealed class PaymentMethod
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public string CardType { get; set; } = string.Empty;

    public string CardNumber { get; set; } = string.Empty;

    public string Expiry { get; set; } = string.Empty;

    public bool IsDefault { get; set; }

    public string? GatewayToken { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public User User { get; set; } = null!;
}
