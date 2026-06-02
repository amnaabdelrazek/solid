<?php

namespace Modules\Auth\Enums;

use App\Support\Traits\EnumCommonTrait;

enum DeviceEventType: string
{
    use EnumCommonTrait;

    case Login = 'login';
    case Logout = 'logout';
    case Forced = 'forced_logout';
}
