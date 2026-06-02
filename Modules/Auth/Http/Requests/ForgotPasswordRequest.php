<?php

namespace Modules\Auth\Http\Requests;

use App\Http\Requests\BaseRequest;

class ForgotPasswordRequest extends BaseRequest
{
    public function rules(): array
    {
        return [
            'mobile_number' => ['required', 'string'],
        ];
    }
}
