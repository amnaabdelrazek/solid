<?php

namespace App\Filament\Resources\Lookups\Pages;

use App\Filament\Resources\Lookups\LookupTypeResource;
use Filament\Actions\DeleteAction;
use Filament\Resources\Pages\EditRecord;

class EditLookupType extends EditRecord
{
    protected static string $resource = LookupTypeResource::class;

    protected function getHeaderActions(): array
    {
        return [
            DeleteAction::make(),
        ];
    }
}
