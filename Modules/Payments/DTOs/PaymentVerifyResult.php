<?php

namespace Modules\Payments\DTOs;

use Modules\Payments\Enums\PaymentStatus;

final readonly class PaymentVerifyResult
{
    public function __construct(
        public PaymentStatus $status,
        public string $transactionId,
        public array $rawPayload = [],
    ) {}
}
