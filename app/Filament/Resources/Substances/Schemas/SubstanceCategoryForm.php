<?php

namespace App\Filament\Resources\Substances\Schemas;

use Filament\Forms\Components\TextInput;
use Filament\Forms\Components\Toggle;
use Filament\Schemas\Components\Grid;
use Filament\Schemas\Components\Section;
use Filament\Schemas\Schema;
use Illuminate\Support\Str;

class SubstanceCategoryForm
{
    public static function configure(Schema $schema): Schema
    {
        return $schema
            ->components([
                Grid::make(2)
                    ->schema([
                        Section::make('Names')
                            ->description('Category names in both languages.')
                            ->icon('heroicon-o-language')
                            ->schema([
                                TextInput::make('name_ar')
                                    ->label('Name (Arabic)')
                                    ->required()
                                    ->maxLength(255)
                                    ->live(onBlur: true)
                                    ->afterStateUpdated(fn ($state, $set) => $set('slug', Str::slug($state))),
                                TextInput::make('name_en')
                                    ->label('Name (English)')
                                    ->required()
                                    ->maxLength(255)
                                    ->live(onBlur: true)
                                    ->afterStateUpdated(fn ($state, $set) => $set('slug', Str::slug($state))),
                            ])
                            ->columnSpan(1),

                        Section::make('Settings')
                            ->description('Visibility and ordering.')
                            ->icon('heroicon-o-cog-6-tooth')
                            ->schema([
                                TextInput::make('slug')
                                    ->required()
                                    ->unique(ignoreRecord: true)
                                    ->maxLength(255)
                                    ->helperText('Auto-generated from the name, but can be customized.'),
                                TextInput::make('sort_order')
                                    ->numeric()
                                    ->default(0)
                                    ->minValue(0)
                                    ->label('Sort Order'),
                                Toggle::make('is_active')
                                    ->default(true)
                                    ->inline(false),
                            ])
                            ->columnSpan(1),
                    ]),
            ]);
    }
}
