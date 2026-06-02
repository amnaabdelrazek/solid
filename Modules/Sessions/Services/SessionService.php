<?php

namespace Modules\Sessions\Services;

use App\Support\Traits\MakeAble;
use Illuminate\Support\Facades\Cache;
use Modules\Groups\Repositories\GroupMemberRepositoryInterface;
use Modules\Payments\Enums\PaymentStatus;
use Modules\Payments\Repositories\PaymentRepositoryInterface;
use Modules\Sessions\DTOs\JoinSessionResult;
use Modules\Sessions\Enums\SessionStatus;
use Modules\Sessions\Enums\SessionType;
use Modules\Sessions\Events\SessionEndedEvent;
use Modules\Sessions\Events\SessionStartedEvent;
use Modules\Sessions\Exceptions\PaymentRequiredException;
use Modules\Sessions\Exceptions\PreviousSessionsNotAttendedException;
use Modules\Sessions\Exceptions\SessionNotLiveException;
use Modules\Sessions\Exceptions\UnauthorizedGroupAccessException;
use Modules\Sessions\Models\Session;
use Modules\Sessions\Models\SessionAttendance;
use Modules\Sessions\Repositories\SessionRepositoryInterface;
use Modules\User\Models\User;

final class SessionService
{
    use MakeAble;

    public function __construct(
        private readonly SessionRepositoryInterface $sessions,
        private readonly GroupMemberRepositoryInterface $members,
        private readonly PaymentRepositoryInterface $payments,
        private readonly JitsiTokenService $jitsi,
    ) {}

    public function joinSession(User $user, int $sessionId): JoinSessionResult
    {
        $session = $this->sessions->findOrFail($sessionId);

        $this->assertSessionIsLive($session);
        $this->assertUserIsMember($user, $session);

        if (! $user->isInstructor()) {
            $this->assertPaymentIfRequired($user, $session);
            //  $this->assertUserAttendedPreviousSessions($user, $session);
        }

        $jwt = $this->resolveJitsiToken($user, $session);
        $this->recordAttendance($session, $user);

        return new JoinSessionResult(
            jitsiJwt: $jwt,
            roomName: $session->jitsi_room_name,
            serverUrl: config('jitsi.server_url'),
            durationMinutes: $session->duration_minutes,
        );
    }

    public function listSessions(User $user): array
    {
        if ($user->isInstructor()) {
            $sessions = Session::query()
                ->with(['instructor', 'group'])
                ->where('instructor_id', $user->id)
                ->where('scheduled_at', '>=', now()->subMinutes(45))
                ->where('status', '!=', SessionStatus::Finished)
                ->orderBy('scheduled_at')
                ->get();

            return ['sessions' => $sessions];
        }

        $group = $user->groups()->latest('pivot_joined_at')->first();

        if (! $group) {
            return ['sessions' => collect()];
        }

        $lastAttendedSessionNumber = SessionAttendance::query()
            ->where('user_id', $user->id)
            ->whereHas('session', fn ($q) => $q->where('group_id', $group->id))
            ->where('was_present', true)
            ->join('therapy_sessions', 'session_attendances.session_id', '=', 'therapy_sessions.id')
            ->max('therapy_sessions.session_number') ?? 0;

        $firstUnpaidSession = Session::query()
            ->where('group_id', $group->id)
            ->where('session_type', SessionType::Paid)
            ->whereDoesntHave('payments', fn ($q) => $q->where('user_id', $user->id)->where('status', PaymentStatus::Paid))
            ->orderBy('scheduled_at')
            ->first();

        $maxSessionNumber = $firstUnpaidSession
            ? $firstUnpaidSession->session_number
            : PHP_INT_MAX;

        $sessions = Session::query()
            ->with(['instructor', 'group'])
            ->where('group_id', $group->id)
            ->where('session_number', '>', $lastAttendedSessionNumber)
            ->where('session_number', '<=', $maxSessionNumber)
            ->where('status', '!=', SessionStatus::Finished)
            ->orderBy('session_number')
            ->get();

        return [
            'sessions' => $sessions,
            'next_session_number' => $lastAttendedSessionNumber + 1,
        ];
    }

    public function startSession(Session $session): void
    {
        $this->sessions->updateStatus($session, SessionStatus::Live->value);
        $this->sessions->recordStart($session);
        SessionStartedEvent::dispatch($session);
    }

    public function endSession(Session $session): void
    {
        $this->sessions->updateStatus($session, SessionStatus::Finished->value);
        $this->sessions->recordEnd($session);
        $this->clearJitsiCache($session->id);
        SessionEndedEvent::dispatch($session);
    }

    private function assertSessionIsLive(Session $session): void
    {
        throw_unless($session->status === SessionStatus::Live, SessionNotLiveException::class);
    }

    private function assertUserIsMember(User $user, Session $session): void
    {
        throw_unless(
            $this->members->isMember($session->group_id, $user->id),
            UnauthorizedGroupAccessException::class,
        );
    }

    private function assertPaymentIfRequired(User $user, Session $session): void
    {
        if ($session->session_type === SessionType::Paid) {
            throw_unless(
                $this->payments->hasPaidForSession($user->id, $session->id),
                PaymentRequiredException::class,
            );
        }
    }

    private function assertUserAttendedPreviousSessions(User $user, Session $session): void
    {
        if ($session->session_number === null || $session->session_number <= 1) {
            return;
        }

        $previousSessionsCount = Session::query()
            ->where('group_id', $session->group_id)
            ->where('session_number', '<', $session->session_number)
            ->count();

        $attendedCount = SessionAttendance::query()
            ->where('user_id', $user->id)
            ->whereHas('session', function ($query) use ($session) {
                $query->where('group_id', $session->group_id)
                    ->where('session_number', '<', $session->session_number);
            })
            ->where('was_present', true)
            ->count();

        throw_unless($attendedCount === $previousSessionsCount, new PreviousSessionsNotAttendedException);
    }

    private function resolveJitsiToken(User $user, Session $session): string
    {
        $cacheKey = "jitsi:{$session->id}:{$user->id}";

        return Cache::remember($cacheKey, 3600, fn () => $this->jitsi->generate($session, $user));
    }

    private function recordAttendance(Session $session, User $user): void
    {
        SessionAttendance::updateOrCreate(
            ['session_id' => $session->id, 'user_id' => $user->id],
            ['joined_at' => now(), 'was_present' => true],
        );
    }

    private function clearJitsiCache(int $sessionId): void
    {
        Cache::forget("jitsi:{$sessionId}:*");
    }
}
