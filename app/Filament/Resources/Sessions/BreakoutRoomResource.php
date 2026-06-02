<?php

namespace App\Filament\Resources\Sessions;

use App\Filament\Resources\Sessions\Pages\CreateBreakoutRoom;
use App\Filament\Resources\Sessions\Pages\EditBreakoutRoom;
use App\Filament\Resources\Sessions\Pages\ListBreakoutRooms;
use App\Filament\Resources\Sessions\Schemas\BreakoutRoomForm;
use App\Filament\Resources\Sessions\Tables\BreakoutRoomsTable;
use BackedEnum;
use Filament\Resources\Resource;
use Filament\Schemas\Schema;
use Filament\Support\Icons\Heroicon;
use Filament\Tables\Table;
use Modules\Sessions\Models\BreakoutRoom;

class BreakoutRoomResource extends Resource
{
    protected static ?string $model = BreakoutRoom::class;

    protected static string|BackedEnum|null $navigationIcon = Heroicon::OutlinedVideoCamera;

    protected static ?string $recordTitleAttribute = 'room_name';

    protected static string|\UnitEnum|null $navigationGroup = 'Sessions';

    public static function form(Schema $schema): Schema
    {
        return BreakoutRoomForm::configure($schema);
    }

    public static function table(Table $table): Table
    {
        return BreakoutRoomsTable::configure($table);
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
            'index' => ListBreakoutRooms::route('/'),
            'create' => CreateBreakoutRoom::route('/create'),
            'edit' => EditBreakoutRoom::route('/{record}/edit'),
        ];
    }
}
