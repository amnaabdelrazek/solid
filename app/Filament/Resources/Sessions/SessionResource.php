<?php

namespace App\Filament\Resources\Sessions;

use App\Filament\Resources\Sessions\Pages\CreateSession;
use App\Filament\Resources\Sessions\Pages\EditSession;
use App\Filament\Resources\Sessions\Pages\ListSessions;
use App\Filament\Resources\Sessions\Schemas\SessionForm;
use App\Filament\Resources\Sessions\Tables\SessionsTable;
use BackedEnum;
use Filament\Resources\Resource;
use Filament\Schemas\Schema;
use Filament\Support\Icons\Heroicon;
use Filament\Tables\Table;
use Modules\Sessions\Models\Session;

class SessionResource extends Resource
{
    protected static ?string $model = Session::class;

    protected static string|BackedEnum|null $navigationIcon = Heroicon::OutlinedCalendarDays;

    protected static string|\UnitEnum|null $navigationGroup = 'Sessions';

    protected static ?string $recordTitleAttribute = 'session_number';

    public static function form(Schema $schema): Schema
    {
        return SessionForm::configure($schema);
    }

    public static function table(Table $table): Table
    {
        return SessionsTable::configure($table);
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
            'index' => ListSessions::route('/'),
            'create' => CreateSession::route('/create'),
            'edit' => EditSession::route('/{record}/edit'),
        ];
    }
}
