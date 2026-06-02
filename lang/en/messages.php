<?php

return [
    'validation' => 'Validation failed.',
    'unauthenticated' => 'Unauthenticated.',
    'unauthorized' => 'This action is unauthorized.',
    'model_not_found' => ':model not found.',
    'endpoint_not_found' => 'The requested endpoint does not exist.',
    'method_not_allowed' => 'Method not allowed.',
    'http_error' => 'An HTTP error occurred.',
    'unexpected_error' => 'An unexpected error occurred. Please try again later.',

    'otp' => [
        'expired' => 'OTP has expired. Please request a new one.',
        'invalid' => 'Invalid OTP provided.',
        'sms_body' => 'Your OTP code is: :otp',
    ],

    'password' => [
        'reset_success' => 'Password has been reset successfully.',
        'otp_sent' => 'OTP has been sent to your mobile number.',
    ],
];
