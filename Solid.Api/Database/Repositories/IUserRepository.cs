namespace Solid.Api.Database.Repositories;

public interface IUserRepository
{
    Task<Dictionary<string, object?>?> FindAsync(long userId);

    Task UpdateProfileAsync(long userId, ProfileUpdate update);

    Task<IReadOnlyList<Dictionary<string, object?>>> InstructorsAsync();
}

public sealed record ProfileUpdate(string? DisplayName, string? Email, string? MobileNumber, string? Bio, string? AvatarUrl);
