<?php

namespace Modules\Auth\Http\Requests;

use App\Http\Requests\BaseRequest;
use Illuminate\Validation\Rule;

class RegisterRequest extends BaseRequest
{
    public function rules(): array
    {
        return [
            'display_name' => ['required', 'string', 'max:150'],
            'mobile_number' => ['required', 'string', Rule::unique('users', 'mobile_number')->where('is_active', 1), 'regex:/^\+?[1-9]\d{1,14}$/'],
            'password' => ['required', 'string', 'min:8'],
            'preferred_language' => ['sometimes', 'in:ar,en'],

            'addiction_duration_id' => ['required', 'exists:lookup_values,id'],
            'education_level_id' => ['required', 'exists:lookup_values,id'],
            'had_prior_treatment' => ['required', 'boolean'],
            'substance_ids' => ['required', 'array', 'min:1'],
            'substance_ids.*' => ['exists:substances,id'],
            'treatment_type_ids' => ['sometimes', 'array'],
            'treatment_type_ids.*' => ['exists:lookup_values,id'],
            'addiction_reason' => ['nullable', 'string', 'max:1000'],
            'days_clean' => ['nullable', 'integer', 'min:0'],
        ];
    }
}
