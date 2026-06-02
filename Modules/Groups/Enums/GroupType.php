<?php

namespace Modules\Groups\Enums;

use App\Support\Traits\EnumCommonTrait;

enum GroupType: string
{
    use EnumCommonTrait;

    case SingleCategory = 'single_category';
    case Mixed = 'mixed';
}
