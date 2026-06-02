<?php

namespace App\Filament\Resources\Lookups\Schemas;

use Filament\Forms\Components\TextInput;
use Filament\Schemas\Components\Grid;
use Filament\Schemas\Components\Section;
use Filament\Schemas\Schema;

class LookupTypeForm
{
    public static function configure(Schema $schema): Schema
    {
        return $schema
            ->components([
                Grid::make(2)
                    ->schema([
                        Section::make('Lookup Key')
                            ->description('The unique identifier for this lookup type.')
                            ->icon('heroicon-o-key')
                            ->schema([
                                TextInput::make('key')
                                    ->required()
                                    ->unique(ignoreRecord: true)
                                    ->maxLength(255)
                                    ->helperText('A unique string identifier, e.g. "treatment_type", "education_level"'),
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
            ]);
    }
}
