<?php

namespace Modules\Sessions\Http\Controllers\Api;

use App\Http\Controllers\Api\ApiController;
use Illuminate\Http\JsonResponse;
use Illuminate\Http\Request;
use Illuminate\Support\Str;
use Modules\Sessions\Actions\EndSessionAction;
use Modules\Sessions\Actions\JoinSessionAction;
use Modules\Sessions\Actions\LeaveSessionAction;
use Modules\Sessions\Actions\StartSessionAction;
use Modules\Sessions\Http\Requests\StoreSessionRequest;
use Modules\Sessions\Http\Resources\SessionAttendanceResource;
use Modules\Sessions\Http\Resources\SessionResource;
use Modules\Sessions\Models\Session;
use Modules\Sessions\Models\SessionAttendance;
use Modules\Sessions\Services\JitsiTokenService;
use Modules\Sessions\Services\SessionService;

class SessionController extends ApiController
{
    public function index(Request $request, SessionService $sessionService): JsonResponse
    {
        $result = $sessionService->listSessions($request->user());

        return $this->apiBody($result)->apiResponse();
    }

    public function meUpcomingSessions(Request $request): JsonResponse
    {
        return $this->apiBody(
            [
                'sessions' => SessionResource::collection(
                    Session::query()
                        ->with(['instructor', 'group'])
                        ->whereHas('payments', fn ($q) => $q->where('status', 'paid')->where('user_id', $request->user()->id))
                        ->paginate(10)
                ),
            ]
        )->apiResponse();
    }

    public function store(StoreSessionRequest $request): JsonResponse
    {
        $session = Session::query()->create([
            'group_id' => $request->integer('group_id'),
            'instructor_id' => $request->user()->id,
            'session_type' => $request->string('session_type')->toString(),
            'status' => 'scheduled',
            'scheduled_at' => $request->date('scheduled_at'),
            'duration_minutes' => $request->integer('duration_minutes'),
            'jitsi_room_name' => 'session-'.Str::uuid(),
        ]);

        return $this->apiBody([
            'session' => SessionResource::make($session->load('instructor', 'group')),
        ])->apiMessage('Session created successfully.')->apiResponse();
    }

    public function show(Session $session): JsonResponse
    {
        $session->load('instructor', 'group');

        return $this->apiBody([
            'session' => SessionResource::make($session),
        ])->apiResponse();
    }

    public function join(Request $request, Session $session, JoinSessionAction $action): JsonResponse
    {
        $result = $action->execute($request->user(), $session->id);

        return $this->apiBody(
            $result->toArray()
        )->apiResponse();
    }

    public function leave(Request $request, Session $session, LeaveSessionAction $action): JsonResponse
    {
        $action->execute($request->user(), $session);

        return $this->apiMessage('Left session successfully.')->apiResponse();
    }

    public function start(Request $request, Session $session, StartSessionAction $action): JsonResponse
    {
        $action->execute($session);

        $session->load('instructor', 'group');

        $jitsi = app(JitsiTokenService::class)->generate($session, $request->user());

        return $this->apiBody([
            'session' => SessionResource::make($session),
            'jitsi_jwt' => $jitsi,
            'jitsi_room_name' => $session->jitsi_room_name,
            'jitsi_server_url' => config('jitsi.server_url'),
            'session_duration_minutes' => $session->duration_minutes,
        ])->apiMessage('Session started.')->apiResponse();
    }

    public function end(Session $session, EndSessionAction $action): JsonResponse
    {
        $action->execute($session);

        return $this->apiMessage('Session ended.')->apiResponse();
    }

    public function feedback(Request $request, Session $session): JsonResponse
    {
        $request->validate([
            'rating' => 'required|integer|min:1|max:5',
            'comment' => 'nullable|string',
        ]);

        $attendance = SessionAttendance::query()
            ->where('session_id', $session->id)
            ->where('user_id', $request->user()->id)
            ->firstOrFail();

        $attendance->update([
            'rating' => $request->integer('rating'),
            'comment' => $request->string('comment')->toString(),
        ]);

        return $this->apiBody([
            'attendance' => SessionAttendanceResource::make($attendance),
        ])->apiMessage('Feedback submitted successfully.')->apiResponse();
    }
}
