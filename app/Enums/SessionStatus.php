<?php

namespace App\Enums;

enum SessionStatus: string
{
    case SCHEDULED = 'scheduled';
    case LIVE = 'live';
    case FINISHED = 'finished';
    case CANCELLED = 'cancelled';
}
