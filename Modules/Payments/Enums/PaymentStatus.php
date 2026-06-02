<?php

namespace Modules\Payments\Enums;

use App\Support\Traits\EnumCommonTrait;

enum PaymentStatus: string
{
    use EnumCommonTrait;

    case Pending = 'pending';
    case Paid = 'paid';
    case Failed = 'failed';
    case Refunded = 'refunded';
}
