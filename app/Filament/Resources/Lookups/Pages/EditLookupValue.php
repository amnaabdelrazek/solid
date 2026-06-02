<?php

namespace App\Filament\Resources\Lookups\Pages;

use App\Filament\Resources\Lookups\LookupValueResource;
use Filament\Actions\DeleteAction;
use Filament\Resources\Pages\EditRecord;

class EditLookupValue extends EditRecord
{
    protected static string $resource = LookupValueResource::class;

    protected function getHeaderActions(): array
    {
        return [
            DeleteAction::make(),
        ];
    }
}
