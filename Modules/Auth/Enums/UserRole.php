<?php

namespace Modules\Auth\Enums;

use App\Support\Traits\EnumCommonTrait;

enum UserRole: string
{
    use EnumCommonTrait;

    case Addict = 'addict';
    case Instructor = 'instructor';
    case Admin = 'admin';
}
