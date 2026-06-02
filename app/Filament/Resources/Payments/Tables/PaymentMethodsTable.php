<?php

namespace App\Filament\Resources\Payments\Tables;

use Filament\Actions\EditAction;
use Filament\Tables\Columns\IconColumn;
use Filament\Tables\Columns\TextColumn;
use Filament\Tables\Table;

class PaymentMethodsTable
{
    public static function configure(Table $table): Table
    {
        return $table
            ->columns([
                TextColumn::make('user.display_name')
                    ->label('User')
                    ->sortable()
                    ->searchable(),
                TextColumn::make('card_type')
                    ->badge()
                    ->sortable()
                    ->searchable(),
                TextColumn::make('card_number')
                    ->searchable(),
                TextColumn::make('expiry')
                    ->sortable(),
                IconColumn::make('is_default')
                    ->boolean()
                    ->label('Default'),
                TextColumn::make('created_at')
                    ->dateTime()
                    ->sortable()
                    ->toggleable(isToggledHiddenByDefault: true),
            ])
            ->filters([
                //
            ])
            ->actions([
                EditAction::make(),
            ]);
    }
}
