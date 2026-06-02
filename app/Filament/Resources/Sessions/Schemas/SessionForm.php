<?php

namespace App\Filament\Resources\Sessions\Schemas;

use Filament\Forms\Components\DateTimePicker;
use Filament\Forms\Components\Select;
use Filament\Forms\Components\TextInput;
use Filament\Schemas\Components\Grid;
use Filament\Schemas\Components\Section;
use Filament\Schemas\Schema;
use Modules\Sessions\Enums\SessionStatus;
use Modules\Sessions\Enums\SessionType;

class SessionForm
{
    public static function configure(Schema $schema): Schema
    {
        return $schema
            ->components([
                Grid::make(2)
                    ->schema([
                        Section::make('Session Details')
                            ->description('Basic information about the session.')
                            ->icon('heroicon-o-presentation-chart-bar')
                            ->schema([
                                Select::make('group_id')
                                    ->relationship('group', 'name_en')
                                    ->required()
                                    ->searchable()
                                    ->preload()
                                    ->native(false),
                                Select::make('instructor_id')
                                    ->relationship('instructor', 'display_name')
                                    ->required()
                                    ->searchable()
                                    ->preload()
                                    ->native(false),
                                Grid::make(2)
                                    ->schema([
                                        TextInput::make('session_number')
                                            ->numeric()
                                            ->required()
                                            ->prefix('#'),
                                        Select::make('session_type')
                                            ->options(SessionType::class)
                                            ->required()
                                            ->native(false),
                                    ]),
                                Select::make('status')
                                    ->options(SessionStatus::class)
                                    ->required()
                                    ->native(false),
                            ])
                            ->columnSpan(1),

                        Section::make('Timing & Location')
                            ->description('When and where the session takes place.')
                            ->icon('heroicon-o-clock')
                            ->schema([
                                DateTimePicker::make('scheduled_at')
                                    ->required()
                                    ->native(false),
                                Grid::make(2)
                                    ->schema([
                                        DateTimePicker::make('started_at')
                                            ->native(false),
                                        DateTimePicker::make('ended_at')
                                            ->native(false),
                                    ]),
                                TextInput::make('duration_minutes')
                                    ->numeric()
                                    ->suffix('min')
                                    ->prefixIcon('heroicon-o-clock'),
                                TextInput::make('jitsi_room_name')
                                    ->label('Jitsi Room')
                                    ->maxLength(255)
                                    ->prefixIcon('heroicon-o-video-camera'),
                            ])
                            ->columnSpan(1),
                    ]),
            ]);
    }
}
