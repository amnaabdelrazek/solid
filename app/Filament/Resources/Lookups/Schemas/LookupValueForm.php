<?php

namespace App\Filament\Resources\Lookups\Schemas;

use Filament\Forms\Components\Select;
use Filament\Forms\Components\TextInput;
use Filament\Forms\Components\Toggle;
use Filament\Schemas\Components\Grid;
use Filament\Schemas\Components\Section;
use Filament\Schemas\Schema;

class LookupValueForm
{
    public static function configure(Schema $schema): Schema
    {
        return $schema
            ->components([
                Grid::make(2)
                    ->schema([
                        Section::make('Lookup Value')
                            ->description('The value details and its parent type.')
                            ->icon('heroicon-o-bars-3-center-left')
                            ->schema([
                                Select::make('lookup_type_id')
                                    ->relationship('type', 'label_en')
                                    ->searchable()
                                    ->preload()
                                    ->required()
                                    ->native(false)
                                    ->prefixIcon('heroicon-o-list-bullet'),
                                TextInput::make('value_key')
                                    ->required()
                                    ->maxLength(255)
                                    ->helperText('A unique key for this value within its type.'),
                            ])
                            ->columnSpan(1),

                        Section::make('Labels')
                            ->description('Display labels in both languages.')
                            ->icon('heroicon-o-language')
                            ->schema([
                                TextInput::make('label_ar')
                                    ->label('Label (Arabic)')
                                    ->required()
                                    ->maxLength(255),
                                TextInput::make('label_en')
                                    ->label('Label (English)')
                                    ->required()
                                    ->maxLength(255),
                            ])
                            ->columnSpan(1),
                    ]),
                Grid::make(2)
                    ->schema([
                        TextInput::make('sort_order')
                            ->numeric()
                            ->default(0)
                            ->minValue(0)
                            ->prefixIcon('heroicon-o-arrows-up-down'),
                        Toggle::make('is_active')
                            ->default(true)
                            ->inline(false),
                    ]),
            ]);
    }
}
