<?php

namespace Modules\Groups\Repositories;

use Illuminate\Pagination\LengthAwarePaginator;
use Modules\Groups\Enums\GroupType;
use Modules\Groups\Models\Group;

interface GroupRepositoryInterface
{
    public function list(): LengthAwarePaginator;

    public function findOpenGroup(GroupType $type, ?int $categoryId): ?Group;

    public function create(GroupType $type, ?int $categoryId): Group;
}
