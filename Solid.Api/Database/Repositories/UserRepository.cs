using Solid.Api.Features.Shared;

namespace Solid.Api.Database.Repositories;

public sealed class UserRepository(IDatabase database) : IUserRepository
{
    public async Task<Dictionary<string, object?>?> FindAsync(long userId)
    {
        return await database.UserAsync(userId);
    }

    public async Task UpdateProfileAsync(long userId, ProfileUpdate update)
    {
        await database.ExecuteAsync(
            """
            UPDATE users
            SET display_name = COALESCE(@DisplayName, display_name),
                email = COALESCE(@Email, email),
                mobile_number = COALESCE(@MobileNumber, mobile_number),
                bio = COALESCE(@Bio, bio),
                avatar_url = COALESCE(@AvatarUrl, avatar_url),
                updated_at = SYSDATETIME()
            WHERE id = @userId
            """,
            new { userId, update.DisplayName, update.Email, update.MobileNumber, update.Bio, update.AvatarUrl });
    }

    public async Task<IReadOnlyList<Dictionary<string, object?>>> InstructorsAsync()
    {
        return await database.QueryAsync("SELECT * FROM users WHERE [role] = 'instructor' AND deleted_at IS NULL ORDER BY id");
    }
}
