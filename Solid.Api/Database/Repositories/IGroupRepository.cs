namespace Solid.Api.Database.Repositories;

public interface IGroupRepository
{
    Task<IReadOnlyList<Dictionary<string, object?>>> ListAsync();

    Task<Dictionary<string, object?>?> MyGroupAsync(long userId);

    Task<bool> HasActiveMembershipAsync(long userId);

    Task<Dictionary<string, object?>> FindOrCreateOpenAsync();

    Task AddMemberAsync(object groupId, long userId);
}
