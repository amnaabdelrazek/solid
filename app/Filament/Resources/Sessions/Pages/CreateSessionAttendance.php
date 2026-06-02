<?php

namespace App\Filament\Resources\Sessions\Pages;

use App\Filament\Resources\Sessions\SessionAttendanceResource;
use Filament\Resources\Pages\CreateRecord;

class CreateSessionAttendance extends CreateRecord
{
    protected static string $resource = SessionAttendanceResource::class;
}
