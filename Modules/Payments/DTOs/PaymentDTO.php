<?php

namespace Modules\Payments\DTOs;

use Modules\Sessions\Models\Session;
use Modules\User\Models\User;

final readonly class PaymentDTO
{
    public function __construct(
        public User $user,
        public Session $session,
        public float $amount,
        public string $currency = 'EGP',
    ) {}
}
