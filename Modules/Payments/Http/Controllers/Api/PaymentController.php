<?php

namespace Modules\Payments\Http\Controllers\Api;

use App\Http\Controllers\Api\ApiController;
use Illuminate\Http\JsonResponse;
use Illuminate\Http\Request;
use Modules\Payments\Actions\InitiatePaymentAction;
use Modules\Payments\Contracts\PaymentGatewayFactoryInterface;
use Modules\Payments\Http\Requests\StorePaymentMethodRequest;
use Modules\Payments\Http\Resources\PaymentMethodResource;
use Modules\Payments\Http\Resources\PaymentResource;
use Modules\Payments\Jobs\HandlePaymentWebhookJob;
use Modules\Sessions\Models\Session;

class PaymentController extends ApiController
{
    public function initiate(Request $request, Session $session, InitiatePaymentAction $action): JsonResponse
    {
        $result = $action->execute($request->user(), $session);

        return $this->apiBody([
            'payment' => PaymentResource::make($result['payment']),
            'payment_url' => $result['payment_url'],
        ])->apiResponse();
    }

    public function history(Request $request): JsonResponse
    {
        $payments = $request->user()
            ->payments()
            ->with('session')
            ->latest()
            ->paginate(20);

        return $this->apiBody(
            PaymentResource::collection($payments)
        )->apiResponse();
    }

    public function storePaymentMethod(StorePaymentMethodRequest $request): JsonResponse
    {
        $paymentMethod = $request->user()->paymentMethods()->create([
            'card_type' => $request->string('card_holder')->toString(),
            'card_number' => $request->string('card_number')->toString(),
            'expiry' => $request->string('expiry')->toString(),
            'is_default' => $request->boolean('is_default', 0),
            'gateway_token' => $request->string('gateway_token'),
        ]);

        return $this->apiBody([
            'payment_method' => PaymentMethodResource::make($paymentMethod),
        ])->apiMessage('Payment method added successfully.')->apiResponse();
    }

    public function paymentMethods(Request $request): JsonResponse
    {
        return $this->apiBody([
            'payment_methods' => PaymentMethodResource::collection(
                $request->user()->paymentMethods
            ),
        ])->apiResponse();
    }

    public function webhook(Request $request, string $gateway, PaymentGatewayFactoryInterface $gatewayFactory): JsonResponse
    {
        $payload = $request->all();
        $signature = $request->header('X-Signature', $request->get('hmac', ''));

        $gatewayService = $gatewayFactory->make($gateway);

        abort_unless(
            hash_equals($gatewayService->getSignature($payload), $signature),
            403,
            'Invalid webhook signature.',
        );

        HandlePaymentWebhookJob::dispatch($gateway, $payload);

        return $this->apiMessage('accepted')->apiResponse();
    }
}
