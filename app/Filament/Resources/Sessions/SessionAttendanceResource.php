<?php

namespace App\Filament\Resources\Sessions;

use App\Filament\Resources\Sessions\Pages\CreateSessionAttendance;
use App\Filament\Resources\Sessions\Pages\EditSessionAttendance;
use App\Filament\Resources\Sessions\Pages\ListSessionAttendances;
use App\Filament\Resources\Sessions\Schemas\SessionAttendanceForm;
use App\Filament\Resources\Sessions\Tables\SessionAttendancesTable;
use BackedEnum;
use Filament\Resources\Resource;
use Filament\Schemas\Schema;
use Filament\Support\Icons\Heroicon;
use Filament\Tables\Table;
use Modules\Sessions\Models\SessionAttendance;

class SessionAttendanceResource extends Resource
{
    protected static ?string $model = SessionAttendance::class;

    protected static string|BackedEnum|null $navigationIcon = Heroicon::OutlinedClipboardDocumentCheck;

    protected static ?string $recordTitleAttribute = 'id';

    protected static string|\UnitEnum|null $navigationGroup = 'Sessions';

    public static function form(Schema $schema): Schema
    {
        return SessionAttendanceForm::configure($schema);
    }

    public static function table(Table $table): Table
    {
        return SessionAttendancesTable::configure($table);
    }

    public static function getRelations(): array
    {
        return [
            //
        ];
    }

    public static function getPages(): array
    {
        return [
            'index' => ListSessionAttendances::route('/'),
            'create' => CreateSessionAttendance::route('/create'),
            'edit' => EditSessionAttendance::route('/{record}/edit'),
        ];
    }
}
