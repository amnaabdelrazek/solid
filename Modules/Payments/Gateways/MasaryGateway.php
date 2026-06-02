<?php

namespace Modules\Payments\Gateways;

use Illuminate\Support\Str;
use Modules\Payments\Contracts\PaymentGatewayInterface;
use Modules\Payments\DTOs\PaymentDTO;
use Modules\Payments\DTOs\PaymentInitiateResult;
use Modules\Payments\DTOs\PaymentVerifyResult;
use Modules\Payments\Enums\PaymentStatus;

class MasaryGateway implements PaymentGatewayInterface
{
    public function initiate(PaymentDTO $dto): PaymentInitiateResult
    {
        return new PaymentInitiateResult(
            paymentUrl: config('payments.masary.checkout_url', ''),
            transactionId: (string) Str::uuid(),
            rawResponse: ['provider' => 'masary'],
        );
    }

    public function verify(array $webhookPayload): PaymentVerifyResult
    {
        $status = ($webhookPayload['success'] ?? false) ? PaymentStatus::Paid : PaymentStatus::Failed;

        return new PaymentVerifyResult(
            status: $status,
            transactionId: (string) ($webhookPayload['transaction_id'] ?? ''),
            rawPayload: $webhookPayload,
        );
    }

    public function getSignature(array $payload): string
    {
        return hash_hmac('sha256', json_encode($payload), (string) config('payments.masary.secret'));
    }
}
