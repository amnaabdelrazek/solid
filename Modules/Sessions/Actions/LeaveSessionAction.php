<?php

namespace Modules\Sessions\Actions;

use App\Support\Traits\MakeAble;
use Modules\Sessions\Models\Session;
use Modules\Sessions\Models\SessionAttendance;
use Modules\User\Models\User;

final class LeaveSessionAction
{
    use MakeAble;

    public function execute(User $user, Session $session): void
    {
        SessionAttendance::where('session_id', $session->id)
            ->where('user_id', $user->id)
            ->whereNull('left_at')
            ->update(['left_at' => now()]);
    }
}
