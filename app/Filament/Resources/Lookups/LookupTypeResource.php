<?php

namespace App\Filament\Resources\Lookups;

use App\Filament\Resources\Lookups\Pages\CreateLookupType;
use App\Filament\Resources\Lookups\Pages\EditLookupType;
use App\Filament\Resources\Lookups\Pages\ListLookupTypes;
use App\Filament\Resources\Lookups\Schemas\LookupTypeForm;
use App\Filament\Resources\Lookups\Tables\LookupTypesTable;
use BackedEnum;
use Filament\Resources\Resource;
use Filament\Schemas\Schema;
use Filament\Support\Icons\Heroicon;
use Filament\Tables\Table;
use Modules\Lookups\Models\LookupType;

class LookupTypeResource extends Resource
{
    protected static ?string $model = LookupType::class;

    protected static string|BackedEnum|null $navigationIcon = Heroicon::OutlinedListBullet;

    protected static ?string $recordTitleAttribute = 'key';

    protected static string|\UnitEnum|null $navigationGroup = 'Lookups';

    public static function form(Schema $schema): Schema
    {
        return LookupTypeForm::configure($schema);
    }

    public static function table(Table $table): Table
    {
        return LookupTypesTable::configure($table);
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
            'index' => ListLookupTypes::route('/'),
            'create' => CreateLookupType::route('/create'),
            'edit' => EditLookupType::route('/{record}/edit'),
        ];
    }
}
