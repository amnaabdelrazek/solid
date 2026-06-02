<?php

namespace App\Filament\Resources\Sessions\Pages;

use App\Filament\Resources\Sessions\SessionAttendanceResource;
use Filament\Actions\DeleteAction;
use Filament\Resources\Pages\EditRecord;

class EditSessionAttendance extends EditRecord
{
    protected static string $resource = SessionAttendanceResource::class;

    protected function getHeaderActions(): array
    {
        return [
            DeleteAction::make(),
        ];
    }
}
