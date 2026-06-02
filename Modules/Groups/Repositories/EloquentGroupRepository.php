<?php

namespace Modules\Groups\Repositories;

use App\Settings\GeneralSettings;
use Illuminate\Pagination\LengthAwarePaginator;
use Illuminate\Support\Str;
use Modules\Groups\Enums\GroupStatus;
use Modules\Groups\Enums\GroupType;
use Modules\Groups\Models\Group;

final class EloquentGroupRepository implements GroupRepositoryInterface
{
    public function findOpenGroup(GroupType $type, ?int $categoryId): ?Group
    {
        $settings = app(GeneralSettings::class);

        return Group::query()
            ->where('group_type', $type)
            ->where('status', GroupStatus::Forming)
            ->when($categoryId, fn ($q) => $q->where('substance_category_id', $categoryId))
            ->lockForUpdate()
            ->first();
    }

    public function create(GroupType $type, ?int $categoryId): Group
    {
        $settings = app(GeneralSettings::class);

        return Group::create([
            'group_type' => $type,
            'status' => GroupStatus::Forming,
            'substance_category_id' => $categoryId,
            'name_ar' => 'مجموعة '.Str::random(6),
            'name_en' => 'Group '.Str::random(6),
            'min_members' => $settings->group_min_members,
            'max_members' => $settings->group_max_members,
        ]);
    }

    public function list(): LengthAwarePaginator
    {
        return Group::query()->paginate();
    }
}
