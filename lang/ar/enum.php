<?php

return [
    'user_role' => [
        'addict' => 'متعافي',
        'instructor' => 'مدرب',
        'admin' => 'مسؤول',
    ],
    'device_event_type' => [
        'login' => 'تسجيل دخول',
        'logout' => 'تسجيل خروج',
        'forced_logout' => 'تسجيل خروج قسري',
    ],
    'group_status' => [
        'forming' => 'قيد التكوين',
        'active' => 'نشط',
        'completed' => 'مكتمل',
        'dissolved' => 'منحل',
    ],
    'group_type' => [
        'single_category' => 'فئة واحدة',
        'mixed' => 'مختلط',
    ],
    'payment_status' => [
        'pending' => 'قيد الانتظار',
        'paid' => 'تم الدفع',
        'failed' => 'فشل الدفع',
        'refunded' => 'تم الاسترجاع',
    ],
    'session_status' => [
        'scheduled' => 'مجدول',
        'live' => 'مباشر',
        'finished' => 'منتهي',
        'cancelled' => 'ملغى',
    ],
    'session_type' => [
        'free' => 'مجاني',
        'paid' => 'مدفوع',
    ],
];
