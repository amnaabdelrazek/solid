<?php

namespace Modules\Auth\Actions;

use App\Support\Traits\MakeAble;
use Illuminate\Auth\AuthenticationException;
use Illuminate\Support\Facades\Hash;
use Modules\Auth\DTOs\LoginDTO;
use Modules\Auth\Enums\DeviceEventType;
use Modules\Auth\Services\DeviceSessionService;
use Modules\Notifications\Jobs\NotifyForcedLogoutJob;
use Modules\User\Models\User;

final class LoginAction
{
    use MakeAble;

    public function __construct(
        private readonly DeviceSessionService $deviceService,
    ) {}

    public function execute(LoginDTO $dto): array
    {
        $user = $this->resolveUser($dto->mobileNumber);

        $this->verifyPassword($user, $dto->password);
        $this->handleDeviceConflict($user, $dto->deviceId);

        $token = $user->createToken('device:'.$dto->deviceId);
        $user->update(['active_device_id' => $dto->deviceId]);
        $this->deviceService->log($user, $dto->deviceId, DeviceEventType::Login, $token->accessToken->id);

        return ['token' => $token->plainTextToken, 'user' => $user];
    }

    private function resolveUser(string $mobileNumber): User
    {
        return User::query()->where('mobile_number', $mobileNumber)
            ->where('is_active', true)
            ->firstOrFail();
    }

    private function verifyPassword(User $user, string $password): void
    {
        throw_unless(
            Hash::check($password, $user->password),
            AuthenticationException::class,
        );
    }

    private function handleDeviceConflict(User $user, string $deviceId): void
    {
        if ($user->active_device_id && $user->active_device_id !== $deviceId) {
            $user->tokens()->delete();
            $this->deviceService->log($user, $user->active_device_id, DeviceEventType::Forced);
            NotifyForcedLogoutJob::dispatch($user, $user->active_device_id);
        }
    }
}
