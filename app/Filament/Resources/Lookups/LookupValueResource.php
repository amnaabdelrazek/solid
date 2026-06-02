<?php

namespace App\Filament\Resources\Lookups;

use App\Filament\Resources\Lookups\Pages\CreateLookupValue;
use App\Filament\Resources\Lookups\Pages\EditLookupValue;
use App\Filament\Resources\Lookups\Pages\ListLookupValues;
use App\Filament\Resources\Lookups\Schemas\LookupValueForm;
use App\Filament\Resources\Lookups\Tables\LookupValuesTable;
use BackedEnum;
use Filament\Resources\Resource;
use Filament\Schemas\Schema;
use Filament\Support\Icons\Heroicon;
use Filament\Tables\Table;
use Modules\Lookups\Models\LookupValue;

class LookupValueResource extends Resource
{
    protected static ?string $model = LookupValue::class;

    protected static string|BackedEnum|null $navigationIcon = Heroicon::OutlinedBars3CenterLeft;

    protected static ?string $recordTitleAttribute = 'label_en';

    protected static string|\UnitEnum|null $navigationGroup = 'Lookups';

    public static function form(Schema $schema): Schema
    {
        return LookupValueForm::configure($schema);
    }

    public static function table(Table $table): Table
    {
        return LookupValuesTable::configure($table);
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
            'index' => ListLookupValues::route('/'),
            'create' => CreateLookupValue::route('/create'),
            'edit' => EditLookupValue::route('/{record}/edit'),
        ];
    }
}
