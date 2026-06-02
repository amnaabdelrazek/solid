# Add Payment Method Store Endpoint

## Files to Create

### 1. `Modules/Payments/Http/Requests/StorePaymentMethodRequest.php`

```php
<?php

namespace Modules\Payments\Http\Requests;

use App\Http\Requests\BaseRequest;

class StorePaymentMethodRequest extends BaseRequest
{
    public function rules(): array
    {
        return [
            'card_type' => ['required', 'string', 'in:visa,mastercard,amex'],
            'card_number' => ['required', 'string', 'max:20'],
            'expiry' => ['required', 'string', 'max:10'],
            'is_default' => ['sometimes', 'boolean'],
            'gateway_token' => ['sometimes', 'nullable', 'string'],
        ];
    }
}
```

## Files to Modify

### 2. `Modules/Payments/Http/Controllers/Api/PaymentController.php`

**Add import** after line 10 (`use Modules\Payments\Http\Resources\PaymentMethodResource;`):
```php
use Modules\Payments\Http\Requests\StorePaymentMethodRequest;
```

**Add method** after `initiate()` (before `history()`):
```php
public function storePaymentMethod(StorePaymentMethodRequest $request): JsonResponse
{
    $paymentMethod = $request->user()->paymentMethods()->create([
        'card_type' => $request->string('card_type')->toString(),
        'card_number' => $request->string('card_number')->toString(),
        'expiry' => $request->string('expiry')->toString(),
        'is_default' => $request->boolean('is_default'),
        'gateway_token' => $request->string('gateway_token'),
    ]);

    return $this->apiBody([
        'payment_method' => PaymentMethodResource::make($paymentMethod),
    ])->apiMessage('Payment method added successfully.')->apiResponse();
}
```

### 3. `Modules/Payments/routes/api.php`

Add before line 12:
```php
    Route::post('payment-methods', [PaymentController::class, 'storePaymentMethod']);
    Route::get('payment-methods', [PaymentController::class, 'paymentMethods']);
```
