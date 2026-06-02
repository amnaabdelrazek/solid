<?php

namespace Modules\Groups\Enums;

use App\Support\Traits\EnumCommonTrait;

enum GroupStatus: string
{
    use EnumCommonTrait;

    case Forming = 'forming';
    case Active = 'active';
    case Completed = 'completed';
    case Dissolved = 'dissolved';
}
