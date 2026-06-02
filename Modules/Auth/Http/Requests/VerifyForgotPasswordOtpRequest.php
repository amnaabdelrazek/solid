<?php

namespace Modules\Auth\Http\Requests;

use App\Http\Requests\BaseRequest;

class VerifyForgotPasswordOtpRequest extends BaseRequest
{
    public function rules(): array
    {
        return [
            'token' => ['required', 'string'],
            'otp' => ['required', 'string'],
        ];
    }

    public function getOtp(): string
    {
        return $this->otp;
    }

    public function getToken(): ?string
    {
        return $this->token;
    }
}
