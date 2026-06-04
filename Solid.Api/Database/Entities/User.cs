namespace Solid.Api.Database.Entities;

public sealed class User
{
    public long Id { get; set; }

    public string DisplayName { get; set; } = string.Empty;

    public string? MobileNumber { get; set; }

    public string? Email { get; set; }

    public string? Username { get; set; }

    public string Password { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public string PreferredLanguage { get; set; } = "ar";

    public string? FcmToken { get; set; }

    public string? ActiveDeviceId { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime? EmailVerifiedAt { get; set; }

    public string? Bio { get; set; }

    public string? AvatarUrl { get; set; }

    public string? Experience { get; set; }

    public string? Quote { get; set; }

    public string? RememberToken { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public AddictionProfile? AddictionProfile { get; set; }

    public ICollection<UserSubstance> UserSubstances { get; set; } = [];

    public ICollection<UserTreatmentType> UserTreatmentTypes { get; set; } = [];

    public ICollection<DeviceSession> DeviceSessions { get; set; } = [];

    public ICollection<GroupMember> GroupMembers { get; set; } = [];

    public ICollection<TherapySession> InstructorSessions { get; set; } = [];

    public ICollection<SessionAttendance> SessionAttendances { get; set; } = [];

    public ICollection<Payment> Payments { get; set; } = [];

    public ICollection<PaymentMethod> PaymentMethods { get; set; } = [];
}
