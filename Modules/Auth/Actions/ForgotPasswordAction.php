<?php

namespace Modules\Auth\Actions;

use App\Support\Traits\MakeAble;
use Illuminate\Support\Str;
use Modules\User\Models\User;

final class ForgotPasswordAction
{
    use MakeAble;

    public function execute(string $mobileNumber): string
    {
        $user = User::query()->where('mobile_number', $mobileNumber)->firstOrFail();
        $token = Str::random(64);

        cache()->put("password_reset_token:{$token}", $user->id, config('otp.ttl', 300));

        return $token;
    }
}
