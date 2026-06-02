<?php

namespace Modules\Auth\Http\Resources;

use Illuminate\Http\Resources\Json\JsonResource;
use Modules\Groups\Http\Resources\GroupResource;
use Modules\Sessions\Http\Resources\SessionResource;

class UserResource extends JsonResource
{
    public function toArray($request): array
    {
        return [
            'id' => $this->id,
            'display_name' => $this->display_name,
            'email' => $this->email,
            'mobile_number' => $this->mobile_number,
            'username' => $this->username,
            'role' => $this->role,
            'bio' => $this->bio,
            'avatar_url' => $this->avatar_url,
            'preferred_language' => $this->preferred_language,
            'is_active' => $this->is_active,
            'email_verified_at' => $this->email_verified_at,
            'created_at' => $this->created_at,
            'updated_at' => $this->updated_at,
            'payment_methods' => $this->paymentMethods->map(fn ($pm) => [
                'id' => $pm->id,
                'card_type' => $pm->card_type,
                'card_number' => $pm->card_number,
                'expiry' => $pm->expiry,
                'is_default' => $pm->is_default,
            ]),
            'addiction_profile' => new AddictionProfileResource($this->whenLoaded('addictionProfile')),
            'groups' => GroupResource::collection($this->whenLoaded('groups')),
            'sessions' => SessionResource::collection($this->whenLoaded('sessions')),
            'device_sessions' => DeviceSessionResource::collection($this->whenLoaded('deviceSessions')),
        ];
    }
}
