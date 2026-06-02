<?php

namespace Tests\Unit\Groups;

use Illuminate\Support\Facades\Event;
use Mockery;
use Modules\Groups\Enums\GroupType;
use Modules\Groups\Events\GroupReadyEvent;
use Modules\Groups\Models\Group;
use Modules\Groups\Repositories\GroupMemberRepositoryInterface;
use Modules\Groups\Repositories\GroupRepositoryInterface;
use Modules\Groups\Repositories\UserSubstanceRepositoryInterface;
use Modules\Groups\Services\GroupMatchingService;
use Modules\User\Models\User;
use Tests\TestCase;

class GroupMatchingServiceTest extends TestCase
{
    private GroupRepositoryInterface $groupRepo;

    private GroupMemberRepositoryInterface $memberRepo;

    private UserSubstanceRepositoryInterface $substanceRepo;

    private GroupMatchingService $service;

    protected function setUp(): void
    {
        parent::setUp();
        Event::fake();

        $this->groupRepo = Mockery::mock(GroupRepositoryInterface::class);
        $this->memberRepo = Mockery::mock(GroupMemberRepositoryInterface::class);
        $this->substanceRepo = Mockery::mock(UserSubstanceRepositoryInterface::class);

        $this->service = new GroupMatchingService(
            $this->groupRepo,
            $this->memberRepo,
            $this->substanceRepo,
        );
    }

    public function test_single_category_user_assigned_to_matching_group(): void
    {
        $user = User::factory()->make(['id' => 1]);
        $group = Group::factory()->make(['id' => 5, 'min_members' => 10, 'max_members' => 15]);
        $group->setRelation('members', collect([]));

        $this->substanceRepo->shouldReceive('getCategoryIds')->with(1)->andReturn([3]);
        $this->groupRepo->shouldReceive('findOpenGroup')
            ->with(GroupType::SingleCategory, 3)
            ->andReturn($group);
        $this->memberRepo->shouldReceive('add')->with(5, 1)->once();

        // fresh() returns same group with member_count below threshold
        $freshGroup = clone $group;
        $freshGroup->member_count = 5;

        $result = $this->service->match($user);

        $this->assertEquals(5, $result->id);
        Event::assertNotDispatched(GroupReadyEvent::class);
    }

    public function test_mixed_group_created_for_multi_category_user(): void
    {
        $user = User::factory()->make(['id' => 2]);
        $newGroup = Group::factory()->make(['id' => 9, 'min_members' => 10, 'max_members' => 15]);

        $this->substanceRepo->shouldReceive('getCategoryIds')->with(2)->andReturn([1, 3]);
        $this->groupRepo->shouldReceive('findOpenGroup')
            ->with(GroupType::Mixed, null)
            ->andReturn(null);
        $this->groupRepo->shouldReceive('create')
            ->with(GroupType::Mixed, null)
            ->andReturn($newGroup);
        $this->memberRepo->shouldReceive('add')->with(9, 2)->once();

        $result = $this->service->match($user);

        $this->assertEquals(9, $result->id);
    }

    public function test_group_ready_event_dispatched_when_threshold_met(): void
    {
        $user = User::factory()->make(['id' => 3]);
        $group = Group::factory()->make(['id' => 7, 'min_members' => 10, 'max_members' => 15]);

        $this->substanceRepo->shouldReceive('getCategoryIds')->with(3)->andReturn([2]);
        $this->groupRepo->shouldReceive('findOpenGroup')->andReturn($group);
        $this->memberRepo->shouldReceive('add')->once();

        // simulate group now at threshold after add
        $group->member_count = 10;

        $this->service->match($user);

        Event::assertDispatched(GroupReadyEvent::class);
    }
}
