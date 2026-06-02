<?php

namespace Modules\Payments\Http\Requests;

use App\Http\Requests\BaseRequest;

class StorePaymentMethodRequest extends BaseRequest
{
    public function rules(): array
    {
        return [
            'card_holder' => ['required', 'string'],
            'card_number' => ['required', 'string', 'max:20'],
            'expiry' => ['required', 'string', 'max:10'],
            'is_default' => ['sometimes', 'boolean'],
            'gateway_token' => ['sometimes', 'nullable', 'string'],
        ];
    }
}
