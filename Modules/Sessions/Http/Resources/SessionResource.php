<?php

namespace Modules\Sessions\Http\Resources;

use App\Settings\GeneralSettings;
use Illuminate\Http\Resources\Json\JsonResource;
use Modules\Auth\Http\Resources\UserResource;
use Modules\Groups\Http\Resources\GroupResource;
use Modules\Sessions\Enums\SessionStatus;
use Modules\Sessions\Enums\SessionType;

class SessionResource extends JsonResource
{
    public function toArray($request): array
    {
        $settings = app(GeneralSettings::class);

        $user = $request->user();
        $isBooked = $user ? $this->payments()->where('user_id', $user->id)->where('status', 'paid')->exists() : false;
        $isLive = $this->status === SessionStatus::Live;
        $isStarted = $this->started_at !== null;

        return [
            'id' => $this->id,
            'group_id' => $this->group_id,
            'instructor_id' => $this->instructor_id,
            'session_number' => $this->session_number,
            'title' => 'Session '.$this->session_number,
            'session_type' => $this->session_type,
            'session_type_label' => $this->session_type === SessionType::Paid ? 'Paid Session' : 'Group Session',
            'status' => $this->status,
            'scheduled_at' => $this->scheduled_at,
            'date' => $this->scheduled_at?->format('D, M d') ?? null,
            'time' => $this->scheduled_at?->format('h:i A') ?? null,
            'started_at' => $this->started_at,
            'ended_at' => $this->ended_at,
            'duration_minutes' => $this->duration_minutes,
            'jitsi_room_name' => $this->jitsi_room_name,
            'jitsi_jwt_issued_at' => $this->jitsi_jwt_issued_at,
            'session_metadata' => $this->session_metadata,
            'price' => data_get($this->session_metadata, 'price', $settings->session_price),
            'formatted_price' => number_format(data_get($this->session_metadata, 'price', $settings->session_price), 0).' EGP',
            'created_at' => $this->created_at,
            'updated_at' => $this->updated_at,
            'seat_number' => $this->payments()->where('status', 'paid')->count(),
            'seat_total' => $settings->group_max_members,
            'seat_remaining' => $settings->group_max_members - $this->payments()->where('status', 'paid')->count(),
            'occupancy_text' => $this->payments()->where('status', 'paid')->count().' Taken / '.$settings->group_max_members,
            'is_booked' => $isBooked,
            'is_locked' => ! ($isLive || $isStarted),
            'started_ago_text' => $isStarted ? 'The session started '.$this->started_at->diffForHumans() : null,
            'instructor_name' => $this->instructor?->display_name,
            'instructor_photo' => $this->instructor?->avatar_url,
            'group' => new GroupResource($this->whenLoaded('group')),
            'instructor' => new UserResource($this->whenLoaded('instructor')),
            'attendances' => SessionAttendanceResource::collection($this->whenLoaded('attendances')),
            'breakout_rooms' => BreakoutRoomResource::collection($this->whenLoaded('breakoutRooms')),
        ];
    }
}
