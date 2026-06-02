<?php

namespace Modules\Payments\Gateways;

use Illuminate\Support\Facades\Http;
use Modules\Payments\Contracts\PaymentGatewayInterface;
use Modules\Payments\DTOs\PaymentDTO;
use Modules\Payments\DTOs\PaymentInitiateResult;
use Modules\Payments\DTOs\PaymentVerifyResult;
use Modules\Payments\Enums\PaymentStatus;

final class PaymobGateway implements PaymentGatewayInterface
{
    private string $apiKey;

    private string $integrationId;

    private string $iframeId;

    public function __construct()
    {
        $this->apiKey = config('payments.paymob.api_key');
        $this->integrationId = config('payments.paymob.integration_id');
        $this->iframeId = config('payments.paymob.iframe_id');
    }

    public function initiate(PaymentDTO $dto): PaymentInitiateResult
    {
        $authToken = $this->authenticate();
        $orderId = $this->createOrder($authToken, $dto);
        $paymentToken = $this->requestPaymentKey($authToken, $orderId, $dto);

        return new PaymentInitiateResult(
            paymentUrl: "https://accept.paymob.com/api/acceptance/iframes/{$this->iframeId}?payment_token={$paymentToken}",
            transactionId: (string) $orderId,
        );
    }

    public function verify(array $webhookPayload): PaymentVerifyResult
    {
        $status = $webhookPayload['obj']['success'] === true
            ? PaymentStatus::Paid
            : PaymentStatus::Failed;

        return new PaymentVerifyResult(
            status: $status,
            transactionId: (string) $webhookPayload['obj']['order']['id'],
            rawPayload: $webhookPayload,
        );
    }

    public function getSignature(array $payload): string
    {
        $hmacFields = $this->extractHmacFields($payload);

        return hash_hmac('sha512', implode('', $hmacFields), config('payments.paymob.hmac_secret'));
    }

    private function authenticate(): string
    {
        $response = Http::post('https://accept.paymob.com/api/auth/tokens', [
            'api_key' => $this->apiKey,
        ]);

        return $response->json('token');
    }

    private function createOrder(string $token, PaymentDTO $dto): int
    {
        $response = Http::post('https://accept.paymob.com/api/ecommerce/orders', [
            'auth_token' => $token,
            'delivery_needed' => false,
            'amount_cents' => (int) ($dto->amount * 100),
            'currency' => $dto->currency,
        ]);

        return $response->json('id');
    }

    private function requestPaymentKey(string $token, int $orderId, PaymentDTO $dto): string
    {
        $response = Http::post('https://accept.paymob.com/api/acceptance/payment_keys', [
            'auth_token' => $token,
            'amount_cents' => (int) ($dto->amount * 100),
            'expiration' => 3600,
            'order_id' => $orderId,
            'currency' => $dto->currency,
            'integration_id' => $this->integrationId,
            'billing_data' => ['first_name' => $dto->user->display_name, 'last_name' => $dto->user->display_name, 'email' => 'N/A', 'phone_number' => 'N/A', 'country' => 'EG', 'city' => 'N/A', 'street' => 'N/A', 'floor' => 'N/A', 'building' => 'N/A', 'apartment' => 'N/A'],
        ]);

        return $response->json('token');
    }

    private function extractHmacFields(array $payload): array
    {
        $obj = $payload['obj'] ?? [];

        return [
            $obj['amount_cents'] ?? '',
            $obj['created_at'] ?? '',
            $obj['currency'] ?? '',
            $obj['error_occured'] ?? '',
            $obj['has_parent_transaction'] ?? '',
            $obj['id'] ?? '',
            $obj['integration_id'] ?? '',
            $obj['is_3d_secure'] ?? '',
            $obj['is_auth'] ?? '',
            $obj['is_capture'] ?? '',
            $obj['is_refunded'] ?? '',
            $obj['is_standalone_payment'] ?? '',
            $obj['is_voided'] ?? '',
            $obj['order']['id'] ?? '',
            $obj['owner'] ?? '',
            $obj['pending'] ?? '',
            $obj['source_data']['pan'] ?? '',
            $obj['source_data']['sub_type'] ?? '',
            $obj['source_data']['type'] ?? '',
            $obj['success'] ?? '',
        ];
    }
}
