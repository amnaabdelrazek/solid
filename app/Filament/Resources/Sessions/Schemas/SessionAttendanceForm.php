<?php

namespace App\Filament\Resources\Sessions\Schemas;

use Filament\Forms\Components\DateTimePicker;
use Filament\Forms\Components\Select;
use Filament\Forms\Components\Textarea;
use Filament\Forms\Components\TextInput;
use Filament\Forms\Components\Toggle;
use Filament\Schemas\Components\Grid;
use Filament\Schemas\Components\Section;
use Filament\Schemas\Schema;

class SessionAttendanceForm
{
    public static function configure(Schema $schema): Schema
    {
        return $schema
            ->components([
                Grid::make(2)
                    ->schema([
                        Section::make('Attendance')
                            ->description('Session and user assignment.')
                            ->icon('heroicon-o-user-group')
                            ->schema([
                                Select::make('session_id')
                                    ->relationship('session', 'session_number')
                                    ->getOptionLabelFromRecordUsing(fn ($record) => "Session #{$record->session_number} - {$record->group?->name_en}")
                                    ->searchable()
                                    ->preload()
                                    ->required()
                                    ->native(false),
                                Select::make('user_id')
                                    ->relationship('user', 'display_name')
                                    ->searchable()
                                    ->preload()
                                    ->required()
                                    ->native(false),
                                Toggle::make('was_present')
                                    ->label('Present')
                                    ->default(true)
                                    ->inline(false),
                            ])
                            ->columnSpan(1),

                        Section::make('Timing & Feedback')
                            ->description('When they joined/left and their feedback.')
                            ->icon('heroicon-o-clock')
                            ->schema([
                                DateTimePicker::make('joined_at')
                                    ->native(false),
                                DateTimePicker::make('left_at')
                                    ->native(false),
                                TextInput::make('rating')
                                    ->numeric()
                                    ->minValue(1)
                                    ->maxValue(5)
                                    ->step(1)
                                    ->suffix('/ 5')
                                    ->prefixIcon('heroicon-o-star'),
                                Textarea::make('comment')
                                    ->rows(3)
                                    ->columnSpanFull(),
                            ])
                            ->columnSpan(1),
                    ]),
            ]);
    }
}
