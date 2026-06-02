<?php

namespace Modules\Auth\Actions;

use App\Support\Traits\MakeAble;
use Modules\Auth\Enums\DeviceEventType;
use Modules\Auth\Services\DeviceSessionService;
use Modules\User\Models\User;

final class LogoutAction
{
    use MakeAble;

    public function __construct(
        private readonly DeviceSessionService $deviceService,
    ) {}

    public function execute(User $user): void
    {
        $this->deviceService->log($user, $user->active_device_id, DeviceEventType::Logout);
        $user->currentAccessToken()->delete();
        $user->update(['active_device_id' => null]);
    }
}
