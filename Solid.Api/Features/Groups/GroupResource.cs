namespace Solid.Api.Features.Groups;

public static class GroupResource
{
    public static object From(Dictionary<string, object?> group)
    {
        return new
        {
            id = group["id"],
            group_type = group.GetValueOrDefault("group_type"),
            status = group.GetValueOrDefault("status"),
            name_ar = group.GetValueOrDefault("name_ar"),
            name_en = group.GetValueOrDefault("name_en"),
            min_members = group.GetValueOrDefault("min_members"),
            max_members = group.GetValueOrDefault("max_members"),
            created_at = group.GetValueOrDefault("created_at"),
            updated_at = group.GetValueOrDefault("updated_at")
        };
    }
}
