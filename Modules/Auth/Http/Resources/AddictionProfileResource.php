<?php

namespace Modules\Auth\Http\Resources;

use Illuminate\Http\Resources\Json\JsonResource;
use Modules\Lookups\Http\Resources\LookupValueResource;

class AddictionProfileResource extends JsonResource
{
    public function toArray($request): array
    {
        return [
            'id' => $this->id,
            'user_id' => $this->user_id,
            'had_prior_treatment' => $this->had_prior_treatment,
            'addiction_reason' => $this->addiction_reason,
            'days_clean' => $this->days_clean,
            'created_at' => $this->created_at,
            'updated_at' => $this->updated_at,
            'user' => new UserResource($this->whenLoaded('user')),
            'addiction_duration' => new LookupValueResource($this->whenLoaded('addictionDuration')),
            'education_level' => new LookupValueResource($this->whenLoaded('educationLevel')),
        ];
    }
}
