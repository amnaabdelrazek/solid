<?php

namespace App\Filament\Resources\Sessions\Pages;

use App\Filament\Resources\Sessions\BreakoutRoomResource;
use Filament\Actions\DeleteAction;
use Filament\Resources\Pages\EditRecord;

class EditBreakoutRoom extends EditRecord
{
    protected static string $resource = BreakoutRoomResource::class;

    protected function getHeaderActions(): array
    {
        return [
            DeleteAction::make(),
        ];
    }
}
