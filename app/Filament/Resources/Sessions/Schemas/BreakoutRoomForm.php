<?php

namespace App\Filament\Resources\Sessions\Schemas;

use Filament\Forms\Components\Select;
use Filament\Forms\Components\TextInput;
use Filament\Forms\Components\Toggle;
use Filament\Schemas\Components\Grid;
use Filament\Schemas\Components\Section;
use Filament\Schemas\Schema;

class BreakoutRoomForm
{
    public static function configure(Schema $schema): Schema
    {
        return $schema
            ->components([
                Grid::make(2)
                    ->schema([
                        Section::make('Room Details')
                            ->description('Basic room configuration.')
                            ->icon('heroicon-o-video-camera')
                            ->schema([
                                Select::make('session_id')
                                    ->relationship('session', 'session_number')
                                    ->getOptionLabelFromRecordUsing(fn ($record) => "Session #{$record->session_number} - {$record->group?->name_en}")
                                    ->searchable()
                                    ->preload()
                                    ->required()
                                    ->native(false),
                                TextInput::make('room_name')
                                    ->required()
                                    ->unique(ignoreRecord: true)
                                    ->maxLength(255)
                                    ->prefixIcon('heroicon-o-hashtag'),
                                Select::make('created_by')
                                    ->relationship('creator', 'display_name')
                                    ->searchable()
                                    ->preload()
                                    ->required()
                                    ->native(false)
                                    ->prefixIcon('heroicon-o-user'),
                                Toggle::make('is_open')
                                    ->label('Room is open')
                                    ->default(true)
                                    ->inline(false),
                            ])
                            ->columnSpanFull(),
                    ]),
            ]);
    }
}
