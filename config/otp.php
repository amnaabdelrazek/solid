<?php

return [
    'fixed_otp' => env('FIXED_OTP', rand(1111, 9999)),
    'ttl' => env('OTP_TTL', 300),
];
