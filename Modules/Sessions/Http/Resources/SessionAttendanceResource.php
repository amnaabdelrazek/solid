<?php

namespace Modules\Sessions\Http\Resources;

use Illuminate\Http\Resources\Json\JsonResource;
use Modules\Auth\Http\Resources\UserResource;

class SessionAttendanceResource extends JsonResource
{
    public function toArray($request): array
    {
        return [
            'id' => $this->id,
            'session_id' => $this->session_id,
            'user_id' => $this->user_id,
            'joined_at' => $this->joined_at,
            'left_at' => $this->left_at,
            'was_present' => $this->was_present,
            'rating' => $this->rating,
            'comment' => $this->comment,
            'session' => new SessionResource($this->whenLoaded('session')),
            'user' => new UserResource($this->whenLoaded('user')),
        ];
    }
}
