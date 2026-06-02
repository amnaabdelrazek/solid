<?php

namespace Modules\User\Repositories;

use Modules\User\Models\User;

class UserRepository implements UserRepositoryInterface
{
    public function findOrFail(int $id): User
    {
        return User::query()->findOrFail($id);
    }

    public function updateActive(User $user, bool $active): bool
    {
        return $user->update(['is_active' => $active]);
    }
}
