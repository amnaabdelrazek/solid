<?php

namespace Modules\Sessions\Http\Resources;

use Illuminate\Http\Resources\Json\JsonResource;
use Modules\Auth\Http\Resources\UserResource;

class BreakoutRoomResource extends JsonResource
{
    public function toArray($request): array
    {
        return [
            'id' => $this->id,
            'session_id' => $this->session_id,
            'room_name' => $this->room_name,
            'created_by' => $this->created_by,
            'is_open' => $this->is_open,
            'created_at' => $this->created_at,
            'updated_at' => $this->updated_at,
            'session' => new SessionResource($this->whenLoaded('session')),
            'creator' => new UserResource($this->whenLoaded('creator')),
            'members' => BreakoutRoomMemberResource::collection($this->whenLoaded('members')),
        ];
    }
}
