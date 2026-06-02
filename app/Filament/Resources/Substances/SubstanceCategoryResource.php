<?php

namespace App\Filament\Resources\Substances;

use App\Filament\Resources\Substances\Pages\CreateSubstanceCategory;
use App\Filament\Resources\Substances\Pages\EditSubstanceCategory;
use App\Filament\Resources\Substances\Pages\ListSubstanceCategories;
use App\Filament\Resources\Substances\Schemas\SubstanceCategoryForm;
use App\Filament\Resources\Substances\Tables\SubstanceCategoriesTable;
use BackedEnum;
use Filament\Resources\Resource;
use Filament\Schemas\Schema;
use Filament\Support\Icons\Heroicon;
use Filament\Tables\Table;
use Modules\User\Models\SubstanceCategory;

class SubstanceCategoryResource extends Resource
{
    protected static ?string $model = SubstanceCategory::class;

    protected static string|BackedEnum|null $navigationIcon = Heroicon::OutlinedCube;

    protected static ?string $recordTitleAttribute = 'name';

    protected static string|\UnitEnum|null $navigationGroup = 'Substances';

    public static function form(Schema $schema): Schema
    {
        return SubstanceCategoryForm::configure($schema);
    }

    public static function table(Table $table): Table
    {
        return SubstanceCategoriesTable::configure($table);
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
            'index' => ListSubstanceCategories::route('/'),
            'create' => CreateSubstanceCategory::route('/create'),
            'edit' => EditSubstanceCategory::route('/{record}/edit'),
        ];
    }
}
