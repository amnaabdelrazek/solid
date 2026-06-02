<?php

namespace Modules\Groups\Repositories;

interface GroupMemberRepositoryInterface
{
    public function add(int $groupId, int $userId): void;

    public function isMember(int $groupId, int $userId): bool;
}
