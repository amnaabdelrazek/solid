<?php

return [
    'name' => 'Payments',
    'default_gateway' => env('PAYMENTS_DEFAULT_GATEWAY', 'paymob'),
    'gateway' => env('PAYMENTS_DEFAULT_GATEWAY', 'paymob'),
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
    'masary' => [
        'checkout_url' => env('MASARY_CHECKOUT_URL', ''),
        'secret' => env('MASARY_SECRET', ''),
    ],
];
