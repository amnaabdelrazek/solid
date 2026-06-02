<?php

namespace Modules\Auth\Http\Requests;

use App\Http\Requests\BaseRequest;
use Modules\Auth\Contracts\VerifyOtpRequestContract;

class VerifyRegisterOtpRequest extends BaseRequest implements VerifyOtpRequestContract
{
    /**
     * Get the validation rules that apply to the request.
     */
    public function rules(): array
    {
        return [
            'otp' => 'required|string',
        ];
    }

    public function getOtp(): string
    {
        return $this->otp;
    }

    public function getToken(): ?string
    {
        return $this->bearerToken();
    }
}
