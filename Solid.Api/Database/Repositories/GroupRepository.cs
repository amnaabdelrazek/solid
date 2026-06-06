using Microsoft.EntityFrameworkCore;
using Solid.Api.Database.Entities;

namespace Solid.Api.Database.Repositories;

public sealed class GroupRepository(SolidDbContext dbContext) : IGroupRepository
{
    public async Task<IReadOnlyList<Group>> ListAsync()
    {
        return await dbContext.Groups
            .AsNoTracking()
            .Include(group => group.Members)
            .Include(group => group.SubstanceCategory)
            .Where(group => group.DeletedAt == null)
            .OrderByDescending(group => group.Id)
            .ToListAsync();
    }

    public async Task<Group?> MyGroupAsync(long userId)
    {
        return await dbContext.GroupMembers
            .AsNoTracking()
            .Include(member => member.Group.Members)
            .Include(member => member.Group.SubstanceCategory)
            .Where(member => member.UserId == userId && member.IsActive && member.Group.DeletedAt == null)
            .OrderByDescending(member => member.JoinedAt)
            .Select(member => member.Group)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> HasActiveMembershipAsync(long userId)
    {
        return await dbContext.GroupMembers.AnyAsync(member => member.UserId == userId && member.IsActive);
    }

    public async Task<Group?> FindOrCreateForUserSubstanceAsync(long userId)
    {
        var substanceCategory = await dbContext.UserSubstances
            .AsNoTracking()
            .Where(userSubstance => userSubstance.UserId == userId)
            .OrderBy(userSubstance => userSubstance.Id)
            .Select(userSubstance => userSubstance.Substance.SubstanceCategory)
            .FirstOrDefaultAsync();

        if (substanceCategory is null)
        {
            return null;
        }

        var group = await dbContext.Groups
            .Include(group => group.Members)
            .Include(group => group.SubstanceCategory)
            .Where(group =>
                group.Status == "forming" &&
                group.SubstanceCategoryId == substanceCategory.Id &&
                group.DeletedAt == null)
            .Where(group => group.Members.Count(member => member.IsActive) < group.MaxMembers)
            .OrderBy(group => group.Id)
            .FirstOrDefaultAsync();

        if (group is not null)
        {
            return group;
        }

        var now = DateTime.UtcNow;
        group = new Group
        {
            GroupType = "mixed",
            Status = "forming",
            SubstanceCategoryId = substanceCategory.Id,
            NameAr = substanceCategory.NameAr,
            NameEn = $"{substanceCategory.NameEn} Group",
            MinMembers = 7,
            MaxMembers = 15,
            CreatedAt = now,
            UpdatedAt = now
        };

        dbContext.Groups.Add(group);
        await dbContext.SaveChangesAsync();

        group.SubstanceCategory = substanceCategory;

        return group;
    }

    public async Task AddMemberAsync(long groupId, long userId)
    {
        dbContext.GroupMembers.Add(new GroupMember
        {
            GroupId = groupId,
            UserId = userId,
            JoinedAt = DateTime.UtcNow,
            IsActive = true
        });

        await dbContext.SaveChangesAsync();
    }
}
