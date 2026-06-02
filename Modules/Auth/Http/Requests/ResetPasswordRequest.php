<?php

namespace Modules\Auth\Http\Requests;

use App\Http\Requests\BaseRequest;

class ResetPasswordRequest extends BaseRequest
{
    public function rules(): array
    {
        return [
            'reset_token' => ['required', 'string'],
            'password' => ['required', 'string', 'min:8'],
        ];
    }
}
