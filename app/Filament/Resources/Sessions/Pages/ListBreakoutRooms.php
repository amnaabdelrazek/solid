<?php

namespace App\Filament\Resources\Sessions\Pages;

use App\Filament\Resources\Sessions\BreakoutRoomResource;
use Filament\Actions\CreateAction;
use Filament\Resources\Pages\ListRecords;

class ListBreakoutRooms extends ListRecords
{
    protected static string $resource = BreakoutRoomResource::class;

    protected function getHeaderActions(): array
    {
        return [
            CreateAction::make(),
        ];
    }
}
