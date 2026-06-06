using Solid.Api.Database.Entities;

namespace Solid.Api.Database.Repositories;

public interface IUserRepository
{
    Task<User?> FindAsync(long userId);

    Task UpdateProfileAsync(long userId, ProfileUpdate update);

    Task<IReadOnlyList<User>> InstructorsAsync();

    Task<User?> FindInstructorAsync(long userId);

    Task<User?> FindByMobileAsync(string mobileNumber);

    Task<User> CreateInstructorAsync(InstructorCreate create);
}

public sealed record ProfileUpdate(string? DisplayName, string? Email, string? MobileNumber, string? Bio, string? AvatarUrl);

public sealed record InstructorCreate(
    string DisplayName,
    string MobileNumber,
    string PasswordHash,
    string? Email,
    string? Bio,
    string? AvatarUrl,
    string? Experience,
    string? Quote,
    string PreferredLanguage);
