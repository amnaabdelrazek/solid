<?php

namespace Tests\Unit\Sessions;

use Illuminate\Support\Facades\Cache;
use Mockery;
use Modules\Groups\Repositories\GroupMemberRepositoryInterface;
use Modules\Payments\Repositories\PaymentRepositoryInterface;
use Modules\Sessions\Enums\SessionStatus;
use Modules\Sessions\Enums\SessionType;
use Modules\Sessions\Exceptions\PaymentRequiredException;
use Modules\Sessions\Exceptions\SessionNotLiveException;
use Modules\Sessions\Exceptions\UnauthorizedGroupAccessException;
use Modules\Sessions\Models\Session;
use Modules\Sessions\Repositories\SessionRepositoryInterface;
use Modules\Sessions\Services\JitsiTokenService;
use Modules\Sessions\Services\SessionService;
use Modules\User\Models\User;
use Tests\TestCase;

class SessionServiceTest extends TestCase
{
    private SessionRepositoryInterface $sessionRepo;

    private GroupMemberRepositoryInterface $memberRepo;

    private PaymentRepositoryInterface $paymentRepo;

    private JitsiTokenService $jitsiService;

    private SessionService $service;

    protected function setUp(): void
    {
        parent::setUp();

        $this->sessionRepo = Mockery::mock(SessionRepositoryInterface::class);
        $this->memberRepo = Mockery::mock(GroupMemberRepositoryInterface::class);
        $this->paymentRepo = Mockery::mock(PaymentRepositoryInterface::class);
        $this->jitsiService = Mockery::mock(JitsiTokenService::class);

        $this->service = new SessionService(
            $this->sessionRepo,
            $this->memberRepo,
            $this->paymentRepo,
            $this->jitsiService,
        );
    }

    public function test_throws_when_session_not_live(): void
    {
        $this->expectException(SessionNotLiveException::class);

        $session = new Session;
        $session->status = SessionStatus::Scheduled;

        $this->sessionRepo->shouldReceive('findOrFail')->andReturn($session);

        $this->service->joinSession(new User, 1);
    }

    public function test_throws_when_user_not_group_member(): void
    {
        $this->expectException(UnauthorizedGroupAccessException::class);

        $session = new Session;
        $session->status = SessionStatus::Live;
        $session->group_id = 10;

        $this->sessionRepo->shouldReceive('findOrFail')->andReturn($session);
        $this->memberRepo->shouldReceive('isMember')->with(10, 1)->andReturn(false);

        $user = new User;
        $user->id = 1;

        $this->service->joinSession($user, 1);
    }

    public function test_throws_when_paid_session_has_no_payment(): void
    {
        $this->expectException(PaymentRequiredException::class);

        $session = new Session;
        $session->id = 5;
        $session->status = SessionStatus::Live;
        $session->group_id = 10;
        $session->session_type = SessionType::Paid;

        $user = new User;
        $user->id = 1;

        $this->sessionRepo->shouldReceive('findOrFail')->andReturn($session);
        $this->memberRepo->shouldReceive('isMember')->andReturn(true);
        $this->paymentRepo->shouldReceive('hasPaidForSession')->with(1, 5)->andReturn(false);

        $this->service->joinSession($user, 5);
    }

    public function test_returns_jitsi_result_on_successful_join(): void
    {
        Cache::shouldReceive('remember')->andReturn('jwt-token-xyz');

        $session = new Session;
        $session->id = 5;
        $session->status = SessionStatus::Live;
        $session->group_id = 10;
        $session->session_type = SessionType::Free;
        $session->jitsi_room_name = 'room-uuid-abc';
        $session->duration_minutes = 45;

        $user = new User;
        $user->id = 1;

        $this->sessionRepo->shouldReceive('findOrFail')->andReturn($session);
        $this->memberRepo->shouldReceive('isMember')->andReturn(true);

        $result = $this->service->joinSession($user, 5);

        $this->assertEquals('jwt-token-xyz', $result->jitsiJwt);
        $this->assertEquals('room-uuid-abc', $result->roomName);
        $this->assertEquals(45, $result->durationMinutes);
    }
}
