<?php

namespace Modules\Sessions\Http\Requests;

use App\Http\Requests\BaseRequest;

class StoreSessionRequest extends BaseRequest
{
    public function rules(): array
    {
        return [
            'group_id' => ['required', 'integer', 'exists:groups,id'],
            'session_type' => ['required', 'string', 'in:free,paid'],
            'scheduled_at' => ['required', 'date', 'after:now'],
            'duration_minutes' => ['required', 'integer', 'min:15', 'max:180'],
        ];
    }
}
