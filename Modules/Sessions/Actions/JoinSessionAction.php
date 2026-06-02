<?php

namespace Modules\Sessions\Actions;

use App\Support\Traits\MakeAble;
use Modules\Sessions\DTOs\JoinSessionResult;
use Modules\Sessions\Services\SessionService;
use Modules\User\Models\User;

final class JoinSessionAction
{
    use MakeAble;

    public function __construct(
        private readonly SessionService $sessionService,
    ) {}

    public function execute(User $user, int $sessionId): JoinSessionResult
    {
        return $this->sessionService->joinSession($user, $sessionId);
    }
}
