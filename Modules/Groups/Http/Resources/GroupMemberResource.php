<?php

namespace Modules\Groups\Http\Resources;

use Illuminate\Http\Resources\Json\JsonResource;
use Modules\Auth\Http\Resources\UserResource;

class GroupMemberResource extends JsonResource
{
    public function toArray($request): array
    {
        return [
            'id' => $this->id,
            'group_id' => $this->group_id,
            'user_id' => $this->user_id,
            'joined_at' => $this->joined_at,
            'is_active' => $this->is_active,
            'group' => new GroupResource($this->whenLoaded('group')),
            'user' => new UserResource($this->whenLoaded('user')),
        ];
    }
}
