<?php

namespace Modules\Notifications\Services;

use App\Support\Traits\MakeAble;
use Illuminate\Support\Facades\Http;
use Illuminate\Support\Facades\Log;
use Modules\User\Models\User;

final class PushNotificationService
{
    use MakeAble;

    private string $fcmUrl = 'https://fcm.googleapis.com/v1/projects/{project}/messages:send';

    private string $serverKey;

    public function __construct()
    {
        $this->serverKey = config('services.fcm.server_key') ?? 'server_key';
    }

    public function send(User $user, string $title, string $body): void
    {
        if (! $user->fcm_token) {
            return;
        }

        $this->dispatch($user->fcm_token, $title, $body);
    }

    public function sendToDevice(string $token, string $title, string $body): void
    {
        $this->dispatch($token, $title, $body);
    }

    private function dispatch(string $token, string $title, string $body): void
    {
        // FCM is disabled temporarily until Firebase credentials are ready.
        return;

        $response = Http::withToken($this->serverKey)
            ->post('https://fcm.googleapis.com/fcm/send', [
                'to' => $token,
                'notification' => ['title' => $title, 'body' => $body],
                'priority' => 'high',
            ]);

        if ($response->failed()) {
            Log::warning('FCM push failed', ['token' => $token, 'status' => $response->status()]);
        }
    }
}
