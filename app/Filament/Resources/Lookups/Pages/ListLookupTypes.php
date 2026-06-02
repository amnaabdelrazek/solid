<?php

namespace App\Filament\Resources\Lookups\Pages;

use App\Filament\Resources\Lookups\LookupTypeResource;
use Filament\Actions\CreateAction;
use Filament\Resources\Pages\ListRecords;

class ListLookupTypes extends ListRecords
{
    protected static string $resource = LookupTypeResource::class;

    protected function getHeaderActions(): array
    {
        return [
            CreateAction::make(),
        ];
    }
}
