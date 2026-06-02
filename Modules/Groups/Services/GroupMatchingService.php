<?php

namespace Modules\Groups\Services;

use App\Settings\GeneralSettings;
use App\Support\Traits\MakeAble;
use Illuminate\Support\Facades\DB;
use Modules\Groups\Enums\GroupType;
use Modules\Groups\Events\GroupReadyEvent;
use Modules\Groups\Models\Group;
use Modules\Groups\Repositories\GroupMemberRepositoryInterface;
use Modules\Groups\Repositories\GroupRepositoryInterface;
use Modules\Groups\Repositories\UserSubstanceRepositoryInterface;
use Modules\User\Models\User;

final class GroupMatchingService
{
    use MakeAble;

    public function __construct(
        private readonly GroupRepositoryInterface $groups,
        private readonly GroupMemberRepositoryInterface $members,
        private readonly UserSubstanceRepositoryInterface $substances,
    ) {}

    public function match(User $user): Group
    {
        return DB::transaction(function () use ($user) {
            $categoryIds = $this->substances->getCategoryIds($user->id);
            $groupType = $this->resolveGroupType($categoryIds);
            $categoryId = $groupType === GroupType::SingleCategory ? $categoryIds[0] : null;

            $group = $this->groups->findOpenGroup($groupType, $categoryId)
                ?? $this->groups->create($groupType, $categoryId);

            $this->members->add($group->id, $user->id);
            $this->dispatchIfReady($group->fresh());

            return $group;
        });
    }

    private function resolveGroupType(array $categoryIds): GroupType
    {
        return count($categoryIds) === 1
            ? GroupType::SingleCategory
            : GroupType::Mixed;
    }

    private function dispatchIfReady(Group $group): void
    {
        $settings = app(GeneralSettings::class);

        $reachedMinMembers = $group->member_count >= $settings->group_min_members;
        $timeoutPassed = $group->created_at->addMinutes($settings->auto_start_timeout_minutes)->isPast();

        if ($reachedMinMembers || $timeoutPassed) {
            //  GroupReadyEvent::dispatch($group);
        }
    }
}
