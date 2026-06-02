<?php

namespace Modules\Auth\Services;

use Illuminate\Support\Str;
use Modules\Auth\Exceptions\OtpException;
use Modules\Auth\Http\Requests\VerifyForgotPasswordOtpRequest;

class VerifyForgotPasswordOtpService
{
    public function verify(VerifyForgotPasswordOtpRequest $request): string
    {
        $userId = cache("password_reset_token:{$request->getToken()}");

        if (! $userId) {
            throw OtpException::expired();
        }

        $storedOtp = cache("password_reset_otp:{$userId}");

        if (! $storedOtp || $storedOtp != $request->getOtp()) {
            throw OtpException::invalid();
        }

        $resetToken = Str::random(64);
        cache()->put("password_reset_verified:{$resetToken}", $userId, config('otp.ttl', 300));

        cache()->forget("password_reset_token:{$request->getToken()}");
        cache()->forget("password_reset_otp:{$userId}");

        return $resetToken;
    }
}
