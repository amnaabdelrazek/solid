using Solid.Api.Database.Entities;

namespace Solid.Api.Features.Groups;

public static class GroupResource
{
    public static object From(Group group)
    {
        return new
        {
            id = group.Id,
            group_type = group.GroupType,
            status = group.Status,
            name_ar = group.NameAr,
            name_en = group.NameEn,
            min_members = group.MinMembers,
            max_members = group.MaxMembers,
            created_at = group.CreatedAt,
            updated_at = group.UpdatedAt
        };
    }
}
