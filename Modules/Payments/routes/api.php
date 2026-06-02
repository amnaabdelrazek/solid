<?php

use Illuminate\Support\Facades\Route;
use Modules\Payments\Http\Controllers\Api\PaymentController;

Route::post('payments/webhook/{gateway}', [PaymentController::class, 'webhook'])
    ->whereIn('gateway', ['paymob', 'fawry', 'masary']);

Route::middleware('auth:sanctum')->group(function () {
    Route::post('payments/initiate/{session}', [PaymentController::class, 'initiate']);
    Route::get('payments/history', [PaymentController::class, 'history']);
    Route::post('payment-methods', [PaymentController::class, 'storePaymentMethod']);
    Route::get('payment-methods', [PaymentController::class, 'paymentMethods']);
});
