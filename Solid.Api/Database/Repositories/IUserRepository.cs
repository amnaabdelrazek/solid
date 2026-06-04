using Solid.Api.Database.Entities;

namespace Solid.Api.Database.Repositories;

public interface IUserRepository
{
    Task<User?> FindAsync(long userId);

    Task UpdateProfileAsync(long userId, ProfileUpdate update);

    Task<IReadOnlyList<User>> InstructorsAsync();
}

public sealed record ProfileUpdate(string? DisplayName, string? Email, string? MobileNumber, string? Bio, string? AvatarUrl);
