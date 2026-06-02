<?php

namespace Modules\Auth\DTOs;

final readonly class RegisterUserDTO
{
    public function __construct(
        public string $displayName,
        public string $mobileNumber,
        public string $password,
        public string $preferredLanguage = 'ar',
    ) {}

    public static function fromRequest(array $data): self
    {
        return new self(
            displayName: $data['display_name'],
            mobileNumber: $data['mobile_number'],
            password: $data['password'],
            preferredLanguage: $data['preferred_language'] ?? 'ar',
        );
    }
}
