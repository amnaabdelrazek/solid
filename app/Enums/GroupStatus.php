<?php

namespace App\Enums;

enum GroupStatus: string
{
    case FORMING = 'forming';
    case ACTIVE = 'active';
    case COMPLETED = 'completed';
    case DISSOLVED = 'dissolved';
}
