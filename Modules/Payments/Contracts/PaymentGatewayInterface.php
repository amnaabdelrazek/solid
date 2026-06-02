<?php

namespace Modules\Payments\Contracts;

use Modules\Payments\DTOs\PaymentDTO;
use Modules\Payments\DTOs\PaymentInitiateResult;
use Modules\Payments\DTOs\PaymentVerifyResult;

interface PaymentGatewayInterface
{
    public function initiate(PaymentDTO $dto): PaymentInitiateResult;

    public function verify(array $webhookPayload): PaymentVerifyResult;

    public function getSignature(array $payload): string;
}
