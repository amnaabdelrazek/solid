<?php

namespace Modules\Auth\Actions;

use App\Support\Traits\MakeAble;
use Modules\Auth\Exceptions\OtpException;
use Modules\User\Models\User;

final class ResetPasswordAction
{
    use MakeAble;

    public function execute(string $resetToken, string $password): void
    {
        $userId = cache("password_reset_verified:{$resetToken}");

        if (! $userId) {
            throw OtpException::expired();
        }

        $user = User::query()->findOrFail($userId);
        $user->update(['password' => $password]);

        cache()->forget("password_reset_verified:{$resetToken}");
    }
}
