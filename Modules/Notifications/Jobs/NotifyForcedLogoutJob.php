<?php

namespace Modules\Notifications\Jobs;

use Illuminate\Contracts\Queue\ShouldQueue;
use Illuminate\Foundation\Queue\Queueable;
use Modules\Notifications\Services\PushNotificationService;
use Modules\User\Models\User;

class NotifyForcedLogoutJob implements ShouldQueue
{
    use Queueable;

    public int $tries = 3;

    public function __construct(
        public readonly User $user,
        public readonly string $oldDeviceId,
    ) {}

    public function handle(PushNotificationService $service): void
    {
        $service->sendToDevice(
            token: $this->user->fcm_token ?? '',
            title: 'Session Terminated',
            body: 'Your account was accessed from another device. You have been logged out.',
        );
    }
}
