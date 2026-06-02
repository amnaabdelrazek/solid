<?php

namespace App\Filament\Resources\Substances;

use App\Filament\Resources\Substances\Pages\CreateSubstance;
use App\Filament\Resources\Substances\Pages\EditSubstance;
use App\Filament\Resources\Substances\Pages\ListSubstances;
use App\Filament\Resources\Substances\Schemas\SubstanceForm;
use App\Filament\Resources\Substances\Tables\SubstancesTable;
use BackedEnum;
use Filament\Resources\Resource;
use Filament\Schemas\Schema;
use Filament\Support\Icons\Heroicon;
use Filament\Tables\Table;
use Modules\User\Models\Substance;

class SubstanceResource extends Resource
{
    protected static ?string $model = Substance::class;

    protected static string|BackedEnum|null $navigationIcon = Heroicon::OutlinedBeaker;

    protected static ?string $recordTitleAttribute = 'name';

    protected static string|\UnitEnum|null $navigationGroup = 'Substances';

    public static function form(Schema $schema): Schema
    {
        return SubstanceForm::configure($schema);
    }

    public static function table(Table $table): Table
    {
        return SubstancesTable::configure($table);
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
            'index' => ListSubstances::route('/'),
            'create' => CreateSubstance::route('/create'),
            'edit' => EditSubstance::route('/{record}/edit'),
        ];
    }
}
