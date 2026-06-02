<?php

namespace Modules\Groups\Repositories;

use Modules\Groups\Models\Group;

final class EloquentGroupMemberRepository implements GroupMemberRepositoryInterface
{
    public function add(int $groupId, int $userId): void
    {
        $group = Group::findOrFail($groupId);

        $group->members()->syncWithoutDetaching([
            $userId => ['joined_at' => now(), 'is_active' => true],
        ]);
    }

    public function isMember(int $groupId, int $userId): bool
    {
        return Group::where('id', $groupId)
            ->whereHas('members', fn ($q) => $q->where('users.id', $userId))
            ->exists();
    }
}
