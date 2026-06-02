<?php

namespace Modules\Sessions\DTOs;

final readonly class JoinSessionResult
{
    public function __construct(
        public string $jitsiJwt,
        public string $roomName,
        public string $serverUrl,
        public int $durationMinutes,
    ) {}

    public function toArray(): array
    {
        return [
            'jitsi_jwt' => $this->jitsiJwt,
            'jitsi_room_name' => $this->roomName,
            'jitsi_server_url' => $this->serverUrl,
            'session_duration_minutes' => $this->durationMinutes,
        ];
    }
}
