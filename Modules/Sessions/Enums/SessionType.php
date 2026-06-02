<?php

namespace Modules\Sessions\Enums;

use App\Support\Traits\EnumCommonTrait;

enum SessionType: string
{
    use EnumCommonTrait;

    case Free = 'free';
    case Paid = 'paid';
}
