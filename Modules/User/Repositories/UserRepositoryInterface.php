<?php

namespace Modules\User\Repositories;

use Modules\User\Models\User;

interface UserRepositoryInterface
{
    public function findOrFail(int $id): User;

    public function updateActive(User $user, bool $active): bool;
}
