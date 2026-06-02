<?php

namespace App\Filament\Resources\Recommendations\Schemas;

use Filament\Forms\Components\Select;
use Filament\Forms\Components\Textarea;
use Filament\Forms\Components\TextInput;
use Filament\Forms\Components\Toggle;
use Filament\Schemas\Components\Grid;
use Filament\Schemas\Components\Section;
use Filament\Schemas\Schema;
use Modules\Recommendations\Enums\RecommendationType;

class RecommendationForm
{
    public static function configure(Schema $schema): Schema
    {
        return $schema
            ->components([
                Grid::make(2)
                    ->schema([
                        Section::make('Recommendation Details')
                            ->description('Basic information about the recommendation.')
                            ->icon('heroicon-o-information-circle')
                            ->schema([
                                Grid::make(2)
                                    ->schema([
                                        TextInput::make('name_ar')
                                            ->label('Name (Arabic)')
                                            ->required()
                                            ->maxLength(255),
                                        TextInput::make('name_en')
                                            ->label('Name (English)')
                                            ->required()
                                            ->maxLength(255),
                                    ]),
                                Select::make('type')
                                    ->options(RecommendationType::class)
                                    ->required()
                                    ->native(false)
                                    ->prefixIcon('heroicon-o-tag'),
                                Select::make('substance_category_id')
                                    ->relationship('category', 'name_en')
                                    ->searchable()
                                    ->preload()
                                    ->nullable()
                                    ->native(false)
                                    ->prefixIcon('heroicon-o-folder'),
                                Toggle::make('is_active')
                                    ->default(true)
                                    ->inline(false),
                            ])
                            ->columnSpan(1),

                        Section::make('Contact & Location')
                            ->description('How to reach this place.')
                            ->icon('heroicon-o-map-pin')
                            ->schema([
                                Textarea::make('contact_info')
                                    ->rows(3)
                                    ->columnSpanFull(),
                                Grid::make(2)
                                    ->schema([
                                        TextInput::make('latitude')
                                            ->numeric()
                                            ->step(0.000001)
                                            ->prefixIcon('heroicon-o-globe-alt'),
                                        TextInput::make('longitude')
                                            ->numeric()
                                            ->step(0.000001)
                                            ->prefixIcon('heroicon-o-globe-alt'),
                                    ]),
                            ])
                            ->columnSpan(1),
                    ]),
            ]);
    }
}
