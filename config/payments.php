<?php

return [
    'gateway' => env('PAYMENT_GATEWAY', 'paymob'),

    'paymob' => [
        'api_key' => env('PAYMOB_API_KEY'),
        'integration_id' => env('PAYMOB_INTEGRATION_ID'),
        'iframe_id' => env('PAYMOB_IFRAME_ID'),
        'hmac_secret' => env('PAYMOB_HMAC_SECRET'),
    ],

    'fawry' => [
        'merchant_code' => env('FAWRY_MERCHANT_CODE'),
        'security_key' => env('FAWRY_SECURITY_KEY'),
        'base_url' => env('FAWRY_BASE_URL', 'https://www.atfawry.com/ECommerceWeb'),
    ],
];
