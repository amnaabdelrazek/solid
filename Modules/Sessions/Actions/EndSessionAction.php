<?php

namespace Modules\Sessions\Actions;

use App\Support\Traits\MakeAble;
use Modules\Sessions\Enums\SessionStatus;
use Modules\Sessions\Models\Session;
use Modules\Sessions\Services\SessionService;
use Symfony\Component\HttpKernel\Exception\HttpException;

final class EndSessionAction
{
    use MakeAble;

    public function __construct(
        private readonly SessionService $sessionService,
    ) {}

    public function execute(Session $session): void
    {
        throw_if(
            $session->status !== SessionStatus::Live,
            new HttpException(422, 'Only live sessions can be ended.'),
        );

        $this->sessionService->endSession($session);
    }
}
