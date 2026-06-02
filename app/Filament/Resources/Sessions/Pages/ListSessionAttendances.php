<?php

namespace App\Filament\Resources\Sessions\Pages;

use App\Filament\Resources\Sessions\SessionAttendanceResource;
use Filament\Actions\CreateAction;
use Filament\Resources\Pages\ListRecords;

class ListSessionAttendances extends ListRecords
{
    protected static string $resource = SessionAttendanceResource::class;

    protected function getHeaderActions(): array
    {
        return [
            CreateAction::make(),
        ];
    }
}
