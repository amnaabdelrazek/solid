<?php

namespace App\Support\Tenant;

use Stancl\Tenancy\UUIDGenerator;
use Symfony\Component\Uid\Ulid;

class UlidGenerator extends UUIDGenerator
{
    public static function generate($resource): string
    {
        return Ulid::generate();
    }
}
