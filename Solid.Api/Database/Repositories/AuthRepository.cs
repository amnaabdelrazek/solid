using Solid.Api.Features.Shared;

namespace Solid.Api.Database.Repositories;

public sealed class AuthRepository(IDatabase database) : IAuthRepository
{
    public async Task<Dictionary<string, object?>?> FindUserByMobileAsync(string mobileNumber, bool onlyActive = false)
    {
        var activeClause = onlyActive ? "AND is_active = 1" : "";

        return await database.QuerySingleAsync($"SELECT TOP 1 * FROM users WHERE mobile_number = @mobileNumber {activeClause}", new { mobileNumber });
    }

    public async Task<Dictionary<string, object?>> CreateInactiveUserAsync(AuthUserCreate create)
    {
        var userId = await database.ExecuteScalarAsync<long>(
            """
            INSERT INTO users (display_name, mobile_number, [password], [role], preferred_language, is_active, created_at, updated_at)
            OUTPUT INSERTED.id
            VALUES (@DisplayName, @MobileNumber, @PasswordHash, 'addict', @PreferredLanguage, 0, SYSDATETIME(), SYSDATETIME())
            """,
            create);

        await CompleteRegistrationDetailsAsync(userId, create);

        return (await database.UserAsync(userId))!;
    }

    public async Task<bool> HasAddictionProfileAsync(long userId)
    {
        var row = await database.QuerySingleAsync("SELECT TOP 1 id FROM addiction_profiles WHERE user_id = @userId", new { userId });

        return row is not null;
    }

    public async Task CompleteRegistrationDetailsAsync(long userId, AuthUserCreate create)
    {
        if (!await HasAddictionProfileAsync(userId))
        {
            await database.ExecuteAsync(
                """
                INSERT INTO addiction_profiles (user_id, addiction_duration_id, education_level_id, had_prior_treatment, addiction_reason, days_clean, created_at, updated_at)
                VALUES (@userId, @AddictionDurationId, @EducationLevelId, @HadPriorTreatment, @AddictionReason, @DaysClean, SYSDATETIME(), SYSDATETIME())
                """,
                new
                {
                    userId,
                    create.AddictionDurationId,
                    create.EducationLevelId,
                    create.HadPriorTreatment,
                    create.AddictionReason,
                    create.DaysClean
                });
        }

        foreach (var substanceId in create.SubstanceIds.Distinct())
        {
            await database.ExecuteAsync(
                """
                IF NOT EXISTS (SELECT 1 FROM user_substances WHERE user_id = @userId AND substance_id = @substanceId)
                BEGIN
                    INSERT INTO user_substances (user_id, substance_id, created_at)
                    VALUES (@userId, @substanceId, SYSDATETIME())
                END
                """,
                new { userId, substanceId });
        }
    }

    public async Task ActivateUserAsync(long userId)
    {
        await database.ExecuteAsync("UPDATE users SET is_active = 1, updated_at = SYSDATETIME() WHERE id = @userId", new { userId });
    }

    public async Task RecordLoginAsync(long userId, string deviceId)
    {
        await database.ExecuteAsync("UPDATE users SET active_device_id = @deviceId, updated_at = SYSDATETIME() WHERE id = @userId", new { userId, deviceId });
        await database.ExecuteAsync(
            "INSERT INTO device_sessions (user_id, device_id, device_info, event_type, sanctum_token_id, created_at) VALUES (@userId, @deviceId, NULL, 'login', NULL, SYSDATETIME())",
            new { userId, deviceId });
    }

    public async Task DeactivateAccountAsync(long userId)
    {
        await database.ExecuteAsync("UPDATE users SET fcm_token = NULL, active_device_id = NULL, is_active = 0, deleted_at = SYSDATETIME(), updated_at = SYSDATETIME() WHERE id = @userId", new { userId });
    }
}
