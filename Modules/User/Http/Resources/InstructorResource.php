<?php

namespace Modules\User\Http\Resources;

use Illuminate\Http\Resources\Json\JsonResource;

class InstructorResource extends JsonResource
{
    public function toArray($request): array
    {
        return [
            'id' => $this->id,
            'display_name' => $this->display_name,
            'avatar_url' => url($this->getFirstMediaUrl('avatar') ?: $this->avatar_url),
            'bio' => $this->bio,
            'experience' => $this->experience ?? [],
            'quote' => $this->quote,
        ];
    }
}
