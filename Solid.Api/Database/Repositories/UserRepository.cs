using Microsoft.EntityFrameworkCore;
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

    public async Task UpdateProfileAsync(long userId, ProfileUpdate update)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(user => user.Id == userId && user.DeletedAt == null);
        if (user is null)
        {
            return;
        }

        user.DisplayName = update.DisplayName ?? user.DisplayName;
        user.Email = update.Email ?? user.Email;
        user.MobileNumber = update.MobileNumber ?? user.MobileNumber;
        user.Bio = update.Bio ?? user.Bio;
        user.AvatarUrl = update.AvatarUrl ?? user.AvatarUrl;
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
}
