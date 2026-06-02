<?php

namespace App\Filament\Resources\Users\Pages;

use App\Filament\Resources\Users\AddictionProfileResource;
use Filament\Actions\DeleteAction;
use Filament\Resources\Pages\EditRecord;

class EditAddictionProfile extends EditRecord
{
    protected static string $resource = AddictionProfileResource::class;

    protected function getHeaderActions(): array
    {
        return [
            DeleteAction::make(),
        ];
    }
}
