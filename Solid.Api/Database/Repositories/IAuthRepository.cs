namespace Solid.Api.Database.Repositories;

public interface IAuthRepository
{
    Task<Dictionary<string, object?>?> FindUserByMobileAsync(string mobileNumber, bool onlyActive = false);

    Task<Dictionary<string, object?>> CreateInactiveUserAsync(AuthUserCreate create);

    Task<bool> HasAddictionProfileAsync(long userId);

    Task CompleteRegistrationDetailsAsync(long userId, AuthUserCreate create);

    Task ActivateUserAsync(long userId);

    Task RecordLoginAsync(long userId, string deviceId);

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
    string? AddictionReason,
    int? DaysClean);
