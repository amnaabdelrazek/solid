<?php

namespace App\Filament\Resources\Users\Pages;

use App\Filament\Resources\Users\AddictionProfileResource;
use Filament\Actions\CreateAction;
use Filament\Resources\Pages\ListRecords;

class ListAddictionProfiles extends ListRecords
{
    protected static string $resource = AddictionProfileResource::class;

    protected function getHeaderActions(): array
    {
        return [
            CreateAction::make(),
        ];
    }
}
