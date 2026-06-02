<?php

namespace App\Enums;

enum UserRole: string
{
    case ADDICT = 'addict';
    case INSTRUCTOR = 'instructor';
    case ADMIN = 'admin';
}
