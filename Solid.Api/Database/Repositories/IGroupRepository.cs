using Solid.Api.Database.Entities;

namespace Solid.Api.Database.Repositories;

public interface IGroupRepository
{
    Task<IReadOnlyList<Group>> ListAsync();

    Task<Group?> MyGroupAsync(long userId);

    Task<bool> HasActiveMembershipAsync(long userId);

    Task<Group?> FindOrCreateForUserSubstanceAsync(long userId);

    Task AddMemberAsync(long groupId, long userId);
}
