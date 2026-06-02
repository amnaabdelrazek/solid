<?php

namespace App\Enums;

enum DeviceEventType: string
{
    case LOGIN = 'login';
    case LOGOUT = 'logout';
    case FORCED = 'forced_logout';
}
