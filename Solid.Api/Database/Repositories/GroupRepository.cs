using Microsoft.EntityFrameworkCore;
using Solid.Api.Database.Entities;

namespace Solid.Api.Database.Repositories;

public sealed class GroupRepository(SolidDbContext dbContext) : IGroupRepository
{
    public async Task<IReadOnlyList<Group>> ListAsync()
    {
        return await dbContext.Groups
            .AsNoTracking()
            .Where(group => group.DeletedAt == null)
            .OrderByDescending(group => group.Id)
            .ToListAsync();
    }

    public async Task<Group?> MyGroupAsync(long userId)
    {
        return await dbContext.GroupMembers
            .AsNoTracking()
            .Where(member => member.UserId == userId && member.IsActive && member.Group.DeletedAt == null)
            .OrderByDescending(member => member.JoinedAt)
            .Select(member => member.Group)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> HasActiveMembershipAsync(long userId)
    {
        return await dbContext.GroupMembers.AnyAsync(member => member.UserId == userId && member.IsActive);
    }

    public async Task<Group> FindOrCreateOpenAsync()
    {
        var group = await dbContext.Groups
            .Where(group => group.Status == "forming" && group.DeletedAt == null)
            .OrderBy(group => group.Id)
            .FirstOrDefaultAsync();

        if (group is not null)
        {
            return group;
        }

        group = new Group
        {
            GroupType = "mixed",
            Status = "forming",
            NameAr = "مجموعة جديدة",
            NameEn = "New Group",
            MinMembers = 7,
            MaxMembers = 15,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        dbContext.Groups.Add(group);
        await dbContext.SaveChangesAsync();

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
