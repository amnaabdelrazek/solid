<?php

namespace Modules\Sessions\Enums;

use App\Support\Traits\EnumCommonTrait;

enum SessionStatus: string
{
    use EnumCommonTrait;

    case Scheduled = 'scheduled';
    case Live = 'live';
    case Finished = 'finished';
    case Cancelled = 'cancelled';
}
