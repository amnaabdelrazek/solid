<?php

namespace Modules\Notifications\Http\Resources;

use Illuminate\Http\Resources\Json\JsonResource;

class NotificationResource extends JsonResource
{
    public function toArray($request): array
    {
        return [
            'id' => $this->id,
            'title' => $this->data['title'] ?? 'Notification',
            'body' => $this->data['body'] ?? '',
            'type' => $this->data['type'] ?? 'info',
            'icon' => $this->data['icon'] ?? 'bell',
            'read_at' => $this->read_at,
            'created_at' => $this->created_at,
            'created_at_human' => $this->created_at->diffForHumans(),
        ];
    }
}
