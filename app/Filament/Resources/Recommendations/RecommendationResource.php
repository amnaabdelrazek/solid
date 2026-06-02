<?php

namespace App\Filament\Resources\Recommendations;

use App\Filament\Resources\Recommendations\Pages\CreateRecommendation;
use App\Filament\Resources\Recommendations\Pages\EditRecommendation;
use App\Filament\Resources\Recommendations\Pages\ListRecommendations;
use App\Filament\Resources\Recommendations\Schemas\RecommendationForm;
use App\Filament\Resources\Recommendations\Tables\RecommendationsTable;
use BackedEnum;
use Filament\Resources\Resource;
use Filament\Schemas\Schema;
use Filament\Support\Icons\Heroicon;
use Filament\Tables\Table;
use Modules\Recommendations\Models\Recommendation;

class RecommendationResource extends Resource
{
    protected static ?string $model = Recommendation::class;

    protected static string|BackedEnum|null $navigationIcon = Heroicon::OutlinedBuildingOffice2;

    protected static ?string $recordTitleAttribute = 'name_en';

    protected static string|\UnitEnum|null $navigationGroup = 'Recommendations';

    public static function form(Schema $schema): Schema
    {
        return RecommendationForm::configure($schema);
    }

    public static function table(Table $table): Table
    {
        return RecommendationsTable::configure($table);
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
            'index' => ListRecommendations::route('/'),
            'create' => CreateRecommendation::route('/create'),
            'edit' => EditRecommendation::route('/{record}/edit'),
        ];
    }
}
