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
            substance_category_id = group.SubstanceCategoryId,
            substance_category = group.SubstanceCategory is null
                ? null
                : new
                {
                    id = group.SubstanceCategory.Id,
                    name_ar = group.SubstanceCategory.NameAr,
                    name_en = group.SubstanceCategory.NameEn
                },
            name_ar = group.NameAr,
            name_en = group.NameEn,
            min_members = group.MinMembers,
            max_members = group.MaxMembers,
            members_count = group.Members.Count(member => member.IsActive),
            created_at = group.CreatedAt,
            updated_at = group.UpdatedAt
        };
    }
}
