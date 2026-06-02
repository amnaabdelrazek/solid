<?php

namespace Modules\Auth\Http\Requests;

use App\Http\Requests\BaseRequest;

class LoginRequest extends BaseRequest
{
    public function rules(): array
    {
        return [
            'mobile_number' => ['required', 'string'],
            'password' => ['required', 'string'],
            'device_id' => ['required', 'string', 'max:255'],
            'device_info' => ['nullable', 'array'],
        ];
    }
}
