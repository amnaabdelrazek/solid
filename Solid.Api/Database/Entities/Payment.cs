namespace Solid.Api.Database.Entities;

public sealed class Payment
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public long? SessionId { get; set; }

    public decimal Amount { get; set; }

    public string Currency { get; set; } = "EGP";

    public string Status { get; set; } = string.Empty;

    public string Gateway { get; set; } = string.Empty;

    public string? GatewayTransactionId { get; set; }

    public string? GatewayResponse { get; set; }

    public DateTime? PaidAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public User User { get; set; } = null!;

    public TherapySession? Session { get; set; }
}
