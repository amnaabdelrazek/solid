<?php

namespace Modules\Sessions\Http\Resources;

use Illuminate\Http\Resources\Json\JsonResource;
use Modules\Auth\Http\Resources\UserResource;

class BreakoutRoomMemberResource extends JsonResource
{
    public function toArray($request): array
    {
        return [
            'id' => $this->id,
            'breakout_room_id' => $this->breakout_room_id,
            'user_id' => $this->user_id,
            'joined_at' => $this->joined_at,
            'left_at' => $this->left_at,
            'breakout_room' => new BreakoutRoomResource($this->whenLoaded('breakoutRoom')),
            'user' => new UserResource($this->whenLoaded('user')),
        ];
    }
}
