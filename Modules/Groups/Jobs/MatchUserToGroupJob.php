<?php

namespace Modules\Groups\Jobs;

use Illuminate\Contracts\Queue\ShouldQueue;
use Illuminate\Foundation\Queue\Queueable;
use Modules\Groups\Services\GroupMatchingService;
use Modules\User\Models\User;

class MatchUserToGroupJob implements ShouldQueue
{
    use Queueable;

    public int $tries = 3;

    public int $backoff = 30;

    public function __construct(
        public readonly User $user,
    ) {}

    public function handle(GroupMatchingService $service): void
    {
        $service->match($this->user);
    }
}
