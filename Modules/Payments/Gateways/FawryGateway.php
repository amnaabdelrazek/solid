<?php

namespace Modules\Payments\Gateways;

use Illuminate\Support\Facades\Http;
use Illuminate\Support\Str;
use Modules\Payments\Contracts\PaymentGatewayInterface;
use Modules\Payments\DTOs\PaymentDTO;
use Modules\Payments\DTOs\PaymentInitiateResult;
use Modules\Payments\DTOs\PaymentVerifyResult;
use Modules\Payments\Enums\PaymentStatus;

final class FawryGateway implements PaymentGatewayInterface
{
    private string $merchantCode;

    private string $securityKey;

    private string $baseUrl;

    public function __construct()
    {
        $this->merchantCode = config('payments.fawry.merchant_code');
        $this->securityKey = config('payments.fawry.security_key');
        $this->baseUrl = config('payments.fawry.base_url', 'https://www.atfawry.com/ECommerceWeb');
    }

    public function initiate(PaymentDTO $dto): PaymentInitiateResult
    {
        $referenceNumber = Str::uuid()->toString();
        $signature = $this->buildSignature($referenceNumber, $dto);

        $response = Http::post("{$this->baseUrl}/Fawry/api/payment/charge", [
            'merchantCode' => $this->merchantCode,
            'merchantRefNum' => $referenceNumber,
            'customerMobile' => $dto->user->mobile_number,
            'customerEmail' => '',
            'paymentMethod' => 'CARD',
            'amount' => $dto->amount,
            'currencyCode' => $dto->currency,
            'description' => "Session #{$dto->session->id}",
            'signature' => $signature,
        ]);

        return new PaymentInitiateResult(
            paymentUrl: $response->json('nextAction.redirectUrl', ''),
            transactionId: $referenceNumber,
            rawResponse: $response->json(),
        );
    }

    public function verify(array $webhookPayload): PaymentVerifyResult
    {
        $fawryStatus = $webhookPayload['orderStatus'] ?? 'FAILED';

        $status = match ($fawryStatus) {
            'PAID' => PaymentStatus::Paid,
            'REFUNDED' => PaymentStatus::Refunded,
            default => PaymentStatus::Failed,
        };

        return new PaymentVerifyResult(
            status: $status,
            transactionId: $webhookPayload['merchantRefNum'] ?? '',
            rawPayload: $webhookPayload,
        );
    }

    public function getSignature(array $payload): string
    {
        $data = ($payload['merchantCode'] ?? '')
            .($payload['merchantRefNum'] ?? '')
            .($payload['orderAmount'] ?? '')
            .$this->securityKey;

        return hash('sha256', $data);
    }

    private function buildSignature(string $referenceNumber, PaymentDTO $dto): string
    {
        $data = $this->merchantCode
            .$referenceNumber
            .$dto->user->mobile_number
            .number_format($dto->amount, 2, '.', '')
            .$this->securityKey;

        return hash('sha256', $data);
    }
}
