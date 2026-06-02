<?php

namespace Modules\Sessions\Jobs;

use Illuminate\Contracts\Queue\ShouldQueue;
use Illuminate\Foundation\Queue\Queueable;
use Modules\Notifications\Jobs\SendPushNotificationJob;
use Modules\Sessions\Models\Session;

class SendSessionAlertJob implements ShouldQueue
{
    use Queueable;

    public int $tries = 2;

    public function __construct(
        public readonly Session $session,
    ) {}

    public function handle(): void
    {
        $this->session->group
            ->members()
            ->each(function ($user) {
                SendPushNotificationJob::dispatch(
                    $user,
                    'Session Ending Soon',
                    'Your session ends in 5 minutes.',
                );
            });
    }
}
