<?php

namespace App\Filament\Resources\Payments\Schemas;

use Filament\Forms\Components\DateTimePicker;
use Filament\Forms\Components\KeyValue;
use Filament\Forms\Components\Select;
use Filament\Forms\Components\TextInput;
use Filament\Schemas\Components\Grid;
use Filament\Schemas\Components\Section;
use Filament\Schemas\Schema;
use Modules\Payments\Enums\PaymentStatus;

class PaymentForm
{
    public static function configure(Schema $schema): Schema
    {
        return $schema
            ->components([
                Grid::make(2)
                    ->schema([
                        Section::make('Payment Details')
                            ->description('Basic information about the transaction.')
                            ->icon('heroicon-o-credit-card')
                            ->schema([
                                Select::make('user_id')
                                    ->relationship('user', 'display_name')
                                    ->searchable()
                                    ->preload()
                                    ->required()
                                    ->native(false),
                                Select::make('session_id')
                                    ->relationship('session', 'session_number')
                                    ->searchable()
                                    ->preload()
                                    ->nullable()
                                    ->native(false),
                                Grid::make(2)
                                    ->schema([
                                        TextInput::make('amount')
                                            ->numeric()
                                            ->prefixIcon('heroicon-o-currency-dollar')
                                            ->required(),
                                        TextInput::make('currency')
                                            ->required()
                                            ->default('USD')
                                            ->maxLength(3),
                                    ]),
                                Select::make('status')
                                    ->options(PaymentStatus::class)
                                    ->required()
                                    ->native(false),
                            ])
                            ->columnSpan(1),

                        Section::make('Gateway Information')
                            ->description('Details from the payment provider.')
                            ->icon('heroicon-o-building-library')
                            ->schema([
                                TextInput::make('gateway')
                                    ->maxLength(255)
                                    ->placeholder('e.g. Stripe, PayPal'),
                                TextInput::make('gateway_transaction_id')
                                    ->label('Transaction ID')
                                    ->maxLength(255),
                                DateTimePicker::make('paid_at')
                                    ->native(false),
                                KeyValue::make('gateway_response')
                                    ->label('Provider Response')
                                    ->columnSpanFull()
                                    ->disabled()
                                    ->dehydrated(false),
                            ])
                            ->columnSpan(1),
                    ]),
            ]);
    }
}
