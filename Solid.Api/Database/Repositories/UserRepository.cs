using Microsoft.EntityFrameworkCore;
using Solid.Api.Common;
using Solid.Api.Database.Entities;

namespace Solid.Api.Database.Repositories;

public sealed class UserRepository(SolidDbContext dbContext) : IUserRepository
{
    public async Task<User?> FindAsync(long userId)
    {
        return await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Id == userId && user.DeletedAt == null);
    }

    public async Task<User?> FindWithProfileAsync(long userId)
    {
        return await dbContext.Users
            .AsNoTracking()
            .Include(u => u.AddictionProfile)
            .Include(u => u.UserSubstances)
                .ThenInclude(us => us.Substance)
            .FirstOrDefaultAsync(user => user.Id == userId && user.DeletedAt == null);
    }

    public async Task UpdateProfileAsync(long userId, ProfileUpdate update)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(user => user.Id == userId && user.DeletedAt == null);
        if (user is null)
        {
            return;
        }

        // Only update if the new value is not null (preserve existing values)
        if (update.DisplayName != null) user.DisplayName = update.DisplayName;
        if (update.Email != null) user.Email = update.Email;
        if (update.MobileNumber != null) user.MobileNumber = update.MobileNumber;
        if (update.Bio != null) user.Bio = update.Bio;
        if (update.AvatarUrl != null) user.AvatarUrl = update.AvatarUrl;
        user.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<User>> InstructorsAsync()
    {
        return await dbContext.Users
            .AsNoTracking()
            .Where(user => user.Role == "instructor" && user.DeletedAt == null)
            .OrderBy(user => user.Id)
            .ToListAsync();
    }

    public async Task<User?> FindInstructorAsync(long userId)
    {
        return await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Id == userId && user.Role == "instructor" && user.DeletedAt == null);
    }

    public async Task<User?> FindByMobileAsync(string mobileNumber)
    {
        var candidates = PhoneNumberValidator.SearchCandidates(mobileNumber);

        return await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.MobileNumber != null && candidates.Contains(user.MobileNumber) && user.DeletedAt == null);
    }

    public async Task<User> CreateInstructorAsync(InstructorCreate create)
    {
        var now = DateTime.UtcNow;
        var instructor = new User
        {
            DisplayName = create.DisplayName,
            MobileNumber = create.MobileNumber,
            Password = create.PasswordHash,
            Email = create.Email,
            Role = "instructor",
            PreferredLanguage = create.PreferredLanguage,
            Bio = create.Bio,
            AvatarUrl = create.AvatarUrl,
            Experience = create.Experience,
            Quote = create.Quote,
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now
        };

        dbContext.Users.Add(instructor);
        await dbContext.SaveChangesAsync();

        return instructor;
    }
}
