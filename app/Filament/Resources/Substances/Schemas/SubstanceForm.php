<?php

namespace App\Filament\Resources\Substances\Schemas;

use Filament\Forms\Components\Select;
use Filament\Forms\Components\TextInput;
use Filament\Forms\Components\Toggle;
use Filament\Schemas\Components\Grid;
use Filament\Schemas\Components\Section;
use Filament\Schemas\Schema;

class SubstanceForm
{
    public static function configure(Schema $schema): Schema
    {
        return $schema
            ->components([
                Grid::make(2)
                    ->schema([
                        Section::make('Substance Details')
                            ->description('Name and category information.')
                            ->icon('heroicon-o-information-circle')
                            ->schema([
                                Select::make('substance_category_id')
                                    ->relationship('category', 'name_en')
                                    ->searchable()
                                    ->preload()
                                    ->required()
                                    ->native(false)
                                    ->prefixIcon('heroicon-o-folder'),
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
                                Toggle::make('is_active')
                                    ->default(true)
                                    ->inline(false),
                            ])
                            ->columnSpanFull(),
                    ]),
            ]);
    }
}
