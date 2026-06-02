<?php

namespace Modules\Groups\Http\Resources;

use Illuminate\Http\Resources\Json\JsonResource;
use Modules\Auth\Http\Resources\UserResource;
use Modules\Lookups\Http\Resources\SubstanceCategoryResource;
use Modules\Sessions\Http\Resources\SessionResource;

class GroupResource extends JsonResource
{
    public function toArray($request): array
    {
        return [
            'id' => $this->id,
            'group_type' => $this->group_type,
            'status' => $this->status,
            'name_ar' => $this->name_ar,
            'name_en' => $this->name_en,
            'min_members' => $this->min_members,
            'max_members' => $this->max_members,
            'created_at' => $this->created_at,
            'updated_at' => $this->updated_at,
            'instructor' => new UserResource($this->whenLoaded('instructor')),
            'category' => new SubstanceCategoryResource($this->whenLoaded('category')),
            'members' => GroupMemberResource::collection($this->whenLoaded('members')),
            'sessions' => SessionResource::collection($this->whenLoaded('sessions')),
        ];
    }
}
