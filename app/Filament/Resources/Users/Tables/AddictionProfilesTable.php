<?php

namespace App\Filament\Resources\Users\Tables;

use Filament\Actions\BulkActionGroup;
use Filament\Actions\DeleteBulkAction;
use Filament\Actions\EditAction;
use Filament\Tables\Columns\IconColumn;
use Filament\Tables\Columns\TextColumn;
use Filament\Tables\Filters\SelectFilter;
use Filament\Tables\Table;

class AddictionProfilesTable
{
    public static function configure(Table $table): Table
    {
        return $table
            ->columns([
                TextColumn::make('user.display_name')
                    ->label('User')
                    ->icon('heroicon-o-user')
                    ->searchable()
                    ->sortable(),
                TextColumn::make('addictionDuration.label_en')
                    ->label('Duration')
                    ->badge()
                    ->color('warning')
                    ->toggleable(),
                TextColumn::make('educationLevel.label_en')
                    ->label('Education')
                    ->toggleable(),
                IconColumn::make('had_prior_treatment')
                    ->label('Prior Treatment')
                    ->boolean()
                    ->sortable(),
                TextColumn::make('days_clean')
                    ->label('Days Clean')
                    ->badge()
                    ->color(fn ($state) => match (true) {
                        $state >= 30 => 'success',
                        $state >= 7 => 'warning',
                        default => 'danger',
                    })
                    ->sortable(),
                TextColumn::make('created_at')
                    ->dateTime()
                    ->sortable()
                    ->toggleable(isToggledHiddenByDefault: true),
            ])
            ->filters([
                SelectFilter::make('had_prior_treatment')
                    ->label('Prior Treatment')
                    ->options([
                        '1' => 'Yes',
                        '0' => 'No',
                    ]),
                SelectFilter::make('addiction_duration_id')
                    ->label('Duration')
                    ->relationship('addictionDuration', 'label_en')
                    ->searchable()
                    ->preload(),
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
