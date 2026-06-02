<?php

namespace Modules\Auth\DTOs;

final readonly class LoginDTO
{
    public function __construct(
        public string $mobileNumber,
        public string $password,
        public string $deviceId,
        public ?array $deviceInfo = null,
    ) {}

    public static function fromRequest(array $data): self
    {
        return new self(
            mobileNumber: $data['mobile_number'],
            password: $data['password'],
            deviceId: $data['device_id'],
            deviceInfo: $data['device_info'] ?? null,
        );
    }
}
