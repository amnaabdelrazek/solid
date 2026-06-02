<?php

namespace Modules\Sessions\Repositories;

use Modules\Sessions\Models\Session;

final class EloquentSessionRepository implements SessionRepositoryInterface
{
    public function findOrFail(int $id): Session
    {
        return Session::findOrFail($id);
    }

    public function updateStatus(Session $session, string $status): void
    {
        $session->update(['status' => $status]);
    }

    public function recordStart(Session $session): void
    {
        $session->update(['started_at' => now(), 'jitsi_jwt_issued_at' => now()]);
    }

    public function recordEnd(Session $session): void
    {
        $session->update(['ended_at' => now()]);
    }
}
