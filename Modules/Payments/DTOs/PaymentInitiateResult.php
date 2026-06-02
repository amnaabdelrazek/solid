<?php

namespace Modules\Payments\DTOs;

final readonly class PaymentInitiateResult
{
    public function __construct(
        public string $paymentUrl,
        public string $transactionId,
        public array $rawResponse = [],
    ) {}
}
