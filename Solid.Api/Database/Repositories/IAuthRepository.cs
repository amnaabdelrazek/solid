using Solid.Api.Database.Entities;

namespace Solid.Api.Database.Repositories;

public interface IAuthRepository
{
    Task<User?> FindUserByIdAsync(long userId);

    Task<User?> FindUserByMobileAsync(string mobileNumber, bool onlyActive = false);

    Task<User> CreateInactiveUserAsync(AuthUserCreate create);

    Task<bool> HasAddictionProfileAsync(long userId);

    Task CompleteRegistrationDetailsAsync(long userId, AuthUserCreate create);

    Task ActivateUserAsync(long userId);

    Task RecordLoginAsync(long userId, string deviceId);

    Task ClearActiveDeviceAsync(long userId);

    Task UpdatePasswordAsync(long userId, string passwordHash);

    Task DeactivateAccountAsync(long userId);
}

public sealed record AuthUserCreate(
    string DisplayName,
    string MobileNumber,
    string PasswordHash,
    string PreferredLanguage,
    long AddictionDurationId,
    long EducationLevelId,
    bool HadPriorTreatment,
    long[] SubstanceIds,
    long[] TreatmentTypeIds,
    string? AddictionReason,
    int? DaysClean);
