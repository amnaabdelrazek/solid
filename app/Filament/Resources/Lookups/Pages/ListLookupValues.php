<?php

namespace App\Filament\Resources\Lookups\Pages;

use App\Filament\Resources\Lookups\LookupValueResource;
use Filament\Actions\CreateAction;
use Filament\Resources\Pages\ListRecords;

class ListLookupValues extends ListRecords
{
    protected static string $resource = LookupValueResource::class;

    protected function getHeaderActions(): array
    {
        return [
            CreateAction::make(),
        ];
    }
}
