<?php

namespace App\Filament\Resources\Substances\Pages;

use App\Filament\Resources\Substances\SubstanceCategoryResource;
use Filament\Actions\CreateAction;
use Filament\Resources\Pages\ListRecords;

class ListSubstanceCategories extends ListRecords
{
    protected static string $resource = SubstanceCategoryResource::class;

    protected function getHeaderActions(): array
    {
        return [
            CreateAction::make(),
        ];
    }
}
