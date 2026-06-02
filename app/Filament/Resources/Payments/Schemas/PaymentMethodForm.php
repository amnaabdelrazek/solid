<?php

namespace App\Filament\Resources\Payments\Schemas;

use Filament\Forms\Components\Select;
use Filament\Forms\Components\TextInput;
use Filament\Forms\Components\Toggle;
use Filament\Schemas\Components\Grid;
use Filament\Schemas\Components\Section;
use Filament\Schemas\Schema;

class PaymentMethodForm
{
    public static function configure(Schema $schema): Schema
    {
        return $schema
            ->components([
                Grid::make(2)
                    ->schema([
                        Section::make('Card Details')
                            ->schema([
                                Select::make('user_id')
                                    ->relationship('user', 'display_name')
                                    ->searchable()
                                    ->preload()
                                    ->required(),
                                TextInput::make('card_type')
                                    ->required()
                                    ->placeholder('e.g. Visa Card'),
                                TextInput::make('card_number')
                                    ->required()
                                    ->placeholder('**** **** **** 1234'),
                                TextInput::make('expiry')
                                    ->required()
                                    ->placeholder('12/26'),
                                Toggle::make('is_default')
                                    ->label('Default Payment Method'),
                                TextInput::make('gateway_token')
                                    ->label('Gateway Token')
                                    ->disabled()
                                    ->dehydrated(false),
                            ]),
                    ]),
            ]);
    }
}
