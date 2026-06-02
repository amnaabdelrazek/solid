<?php

namespace Modules\Auth\Services;

use App\Support\Traits\MakeAble;
use Modules\Auth\Enums\DeviceEventType;
use Modules\Auth\Models\DeviceSession;
use Modules\User\Models\User;

final class DeviceSessionService
{
    use MakeAble;

    public function log(User $user, ?string $deviceId, DeviceEventType $event, ?int $tokenId = null): void
    {
        DeviceSession::create([
            'user_id' => $user->id,
            'device_id' => $deviceId ?? 'unknown',
            'event_type' => $event,
            'sanctum_token_id' => $tokenId,
            'created_at' => now(),
        ]);
    }
}
