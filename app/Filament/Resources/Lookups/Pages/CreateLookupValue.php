<?php

namespace App\Filament\Resources\Lookups\Pages;

use App\Filament\Resources\Lookups\LookupValueResource;
use Filament\Resources\Pages\CreateRecord;

class CreateLookupValue extends CreateRecord
{
    protected static string $resource = LookupValueResource::class;
}
