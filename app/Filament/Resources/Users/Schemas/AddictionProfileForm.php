<?php

namespace App\Filament\Resources\Users\Schemas;

use Filament\Forms\Components\Select;
use Filament\Forms\Components\Textarea;
use Filament\Forms\Components\TextInput;
use Filament\Forms\Components\Toggle;
use Filament\Schemas\Components\Grid;
use Filament\Schemas\Components\Section;
use Filament\Schemas\Schema;

class AddictionProfileForm
{
    public static function configure(Schema $schema): Schema
    {
        return $schema
            ->components([
                Grid::make(2)
                    ->schema([
                        Section::make('Profile Information')
                            ->description('User addiction profile details.')
                            ->icon('heroicon-o-heart')
                            ->schema([
                                Select::make('user_id')
                                    ->relationship('user', 'display_name')
                                    ->searchable()
                                    ->preload()
                                    ->required()
                                    ->unique(ignoreRecord: true)
                                    ->native(false)
                                    ->prefixIcon('heroicon-o-user'),
                                Select::make('addiction_duration_id')
                                    ->relationship('addictionDuration', 'label_en')
                                    ->searchable()
                                    ->preload()
                                    ->required()
                                    ->native(false)
                                    ->label('Addiction Duration'),
                                Select::make('education_level_id')
                                    ->relationship('educationLevel', 'label_en')
                                    ->searchable()
                                    ->preload()
                                    ->required()
                                    ->native(false)
                                    ->label('Education Level'),
                                Select::make('treatmentTypes')
                                    ->multiple()
                                    ->relationship('treatmentTypes', 'label_en')
                                    ->searchable()
                                    ->preload()
                                    ->native(false)
                                    ->label('Treatment Types'),
                                Toggle::make('had_prior_treatment')
                                    ->label('Had Prior Treatment')
                                    ->inline(false),
                            ])
                            ->columnSpan(1),

                        Section::make('Additional Details')
                            ->description('Recovery journey information.')
                            ->icon('heroicon-o-document-text')
                            ->schema([
                                TextInput::make('days_clean')
                                    ->numeric()
                                    ->minValue(0)
                                    ->suffix('days')
                                    ->prefixIcon('heroicon-o-calendar-days'),
                                Textarea::make('addiction_reason')
                                    ->label('Reason for Addiction')
                                    ->rows(5)
                                    ->columnSpanFull(),
                            ])
                            ->columnSpan(1),
                    ]),
            ]);
    }
}
