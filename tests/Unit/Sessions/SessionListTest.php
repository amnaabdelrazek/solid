<?php

namespace Tests\Unit\Sessions;

use Illuminate\Foundation\Testing\RefreshDatabase;
use Illuminate\Support\Str;
use Mockery;
use Modules\Auth\Enums\UserRole;
use Modules\Groups\Enums\GroupStatus;
use Modules\Groups\Enums\GroupType;
use Modules\Groups\Models\Group;
use Modules\Groups\Repositories\GroupMemberRepositoryInterface;
use Modules\Payments\Repositories\PaymentRepositoryInterface;
use Modules\Sessions\Enums\SessionStatus;
use Modules\Sessions\Enums\SessionType;
use Modules\Sessions\Models\Session;
use Modules\Sessions\Repositories\SessionRepositoryInterface;
use Modules\Sessions\Services\JitsiTokenService;
use Modules\Sessions\Services\SessionService;
use Modules\User\Models\User;
use Tests\TestCase;

class SessionListTest extends TestCase
{
    use RefreshDatabase;

    private SessionService $service;

    protected function setUp(): void
    {
        parent::setUp();

        $this->service = new SessionService(
            Mockery::mock(SessionRepositoryInterface::class),
            Mockery::mock(GroupMemberRepositoryInterface::class),
            Mockery::mock(PaymentRepositoryInterface::class),
            Mockery::mock(JitsiTokenService::class),
        );
    }

    public function test_returns_upcoming_sessions_for_instructor(): void
    {
        $instructor = User::factory()->create(['role' => UserRole::Instructor]);
        $group = Group::create([
            'instructor_id' => $instructor->id,
            'group_type' => GroupType::SingleCategory,
            'status' => GroupStatus::Active,
            'name_ar' => 'مجموعة اختبار',
            'name_en' => 'Test Group',
        ]);

        Session::create([
            'group_id' => $group->id,
            'instructor_id' => $instructor->id,
            'session_type' => SessionType::Free,
            'status' => SessionStatus::Finished,
            'scheduled_at' => now()->subHours(2),
            'duration_minutes' => 45,
            'jitsi_room_name' => (string) Str::uuid(),
        ]);

        Session::create([
            'group_id' => $group->id,
            'instructor_id' => $instructor->id,
            'session_type' => SessionType::Free,
            'status' => SessionStatus::Live,
            'scheduled_at' => now()->subMinutes(30),
            'duration_minutes' => 45,
            'jitsi_room_name' => (string) Str::uuid(),
        ]);

        Session::create([
            'group_id' => $group->id,
            'instructor_id' => $instructor->id,
            'session_type' => SessionType::Free,
            'status' => SessionStatus::Scheduled,
            'scheduled_at' => now()->addDay(),
            'duration_minutes' => 45,
            'jitsi_room_name' => (string) Str::uuid(),
        ]);

        $result = $this->service->listSessions($instructor);

        $this->assertCount(2, $result['sessions']);
        $this->assertArrayNotHasKey('next_session_number', $result);
    }

    public function test_returns_empty_for_instructor_with_no_upcoming_sessions(): void
    {
        $instructor = User::factory()->create(['role' => UserRole::Instructor]);
        $group = Group::create([
            'instructor_id' => $instructor->id,
            'group_type' => GroupType::SingleCategory,
            'status' => GroupStatus::Active,
            'name_ar' => 'مجموعة اختبار',
            'name_en' => 'Test Group',
        ]);

        Session::create([
            'group_id' => $group->id,
            'instructor_id' => $instructor->id,
            'session_type' => SessionType::Free,
            'status' => SessionStatus::Finished,
            'scheduled_at' => now()->subHours(2),
            'duration_minutes' => 45,
            'jitsi_room_name' => (string) Str::uuid(),
        ]);

        $result = $this->service->listSessions($instructor);

        $this->assertCount(0, $result['sessions']);
        $this->assertArrayNotHasKey('next_session_number', $result);
    }

    public function test_returns_sessions_for_regular_user_with_group(): void
    {
        $user = User::factory()->create(['role' => UserRole::Addict]);
        $instructor = User::factory()->create(['role' => UserRole::Instructor]);
        $group = Group::create([
            'instructor_id' => $instructor->id,
            'group_type' => GroupType::SingleCategory,
            'status' => GroupStatus::Active,
            'name_ar' => 'مجموعة اختبار',
            'name_en' => 'Test Group',
        ]);

        $user->groups()->attach($group->id, ['joined_at' => now(), 'is_active' => true]);

        Session::create([
            'group_id' => $group->id,
            'instructor_id' => $instructor->id,
            'session_number' => 1,
            'session_type' => SessionType::Free,
            'status' => SessionStatus::Scheduled,
            'scheduled_at' => now()->addDay(),
            'duration_minutes' => 45,
            'jitsi_room_name' => (string) Str::uuid(),
        ]);

        Session::create([
            'group_id' => $group->id,
            'instructor_id' => $instructor->id,
            'session_number' => 2,
            'session_type' => SessionType::Free,
            'status' => SessionStatus::Scheduled,
            'scheduled_at' => now()->addDays(2),
            'duration_minutes' => 45,
            'jitsi_room_name' => (string) Str::uuid(),
        ]);

        $result = $this->service->listSessions($user);

        $this->assertCount(2, $result['sessions']);
        $this->assertEquals(1, $result['next_session_number']);
    }

    public function test_returns_empty_for_regular_user_with_no_group(): void
    {
        $user = User::factory()->create(['role' => UserRole::Addict]);

        $result = $this->service->listSessions($user);

        $this->assertCount(0, $result['sessions']);
        $this->assertArrayNotHasKey('next_session_number', $result);
    }
}
