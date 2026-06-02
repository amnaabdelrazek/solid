<?php

namespace Modules\Groups\Repositories;

use Modules\User\Models\User;

final class EloquentUserSubstanceRepository implements UserSubstanceRepositoryInterface
{
    public function getCategoryIds(int $userId): array
    {
        return User::findOrFail($userId)
            ->substances()
            ->with('category')
            ->get()
            ->pluck('substance_category_id')
            ->unique()
            ->values()
            ->toArray();
    }
}
