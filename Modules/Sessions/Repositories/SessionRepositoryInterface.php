<?php

namespace Modules\Sessions\Repositories;

use Modules\Sessions\Models\Session;

interface SessionRepositoryInterface
{
    public function findOrFail(int $id): Session;

    public function updateStatus(Session $session, string $status): void;

    public function recordStart(Session $session): void;

    public function recordEnd(Session $session): void;
}
