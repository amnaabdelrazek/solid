<?php

namespace Modules\Notifications\Jobs;

use Illuminate\Contracts\Queue\ShouldQueue;
use Illuminate\Foundation\Queue\Queueable;
use Modules\Notifications\Services\PushNotificationService;
use Modules\User\Models\User;

class SendPushNotificationJob implements ShouldQueue
{
    use Queueable;

    public int $tries = 3;

    public int $backoff = 15;

    public function __construct(
        public readonly User $user,
        public readonly string $title,
        public readonly string $body,
    ) {}

    public function handle(PushNotificationService $service): void
    {
        $service->send($this->user, $this->title, $this->body);
    }
}
