<?php

namespace App\Filament\Resources\Payments\Tables;

use Filament\Actions\BulkActionGroup;
use Filament\Actions\DeleteBulkAction;
use Filament\Actions\EditAction;
use Filament\Tables\Columns\TextColumn;
use Filament\Tables\Filters\SelectFilter;
use Filament\Tables\Table;
use Modules\Payments\Enums\PaymentStatus;

class PaymentsTable
{
    public static function configure(Table $table): Table
    {
        return $table
            ->columns([
                TextColumn::make('user.display_name')
                    ->label('Customer')
                    ->icon('heroicon-o-user')
                    ->searchable()
                    ->sortable(),
                TextColumn::make('amount')
                    ->money(fn ($record) => $record->currency)
                    ->color('success')
                    ->weight('bold')
                    ->sortable(),
                TextColumn::make('status')
                    ->badge()
                    ->color(fn (PaymentStatus $state): string => match ($state) {
                        PaymentStatus::Paid => 'success',
                        PaymentStatus::Pending => 'warning',
                        PaymentStatus::Failed => 'danger',
                        PaymentStatus::Refunded => 'gray',
                    })
                    ->searchable(),
                TextColumn::make('gateway')
                    ->label('Provider')
                    ->description(fn ($record) => $record->gateway_transaction_id)
                    ->searchable(),
                TextColumn::make('paid_at')
                    ->label('Paid On')
                    ->dateTime('M d, Y H:i')
                    ->sortable(),
                TextColumn::make('created_at')
                    ->dateTime()
                    ->sortable()
                    ->toggleable(isToggledHiddenByDefault: true),
            ])
            ->filters([
                SelectFilter::make('status')
                    ->options(PaymentStatus::class),
                SelectFilter::make('gateway')
                    ->searchable(),
            ])
            ->recordActions([
                EditAction::make(),
            ])
            ->bulkActions([
                BulkActionGroup::make([
                    DeleteBulkAction::make(),
                ]),
            ]);
    }
}
