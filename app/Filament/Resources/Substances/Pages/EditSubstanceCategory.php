<?php

namespace App\Filament\Resources\Substances\Pages;

use App\Filament\Resources\Substances\SubstanceCategoryResource;
use Filament\Actions\DeleteAction;
use Filament\Resources\Pages\EditRecord;

class EditSubstanceCategory extends EditRecord
{
    protected static string $resource = SubstanceCategoryResource::class;

    protected function getHeaderActions(): array
    {
        return [
            DeleteAction::make(),
        ];
    }
}
