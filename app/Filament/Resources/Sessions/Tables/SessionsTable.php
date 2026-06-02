<?php

namespace App\Filament\Resources\Sessions\Tables;

use Filament\Actions\BulkActionGroup;
use Filament\Actions\DeleteBulkAction;
use Filament\Actions\EditAction;
use Filament\Tables\Columns\TextColumn;
use Filament\Tables\Filters\SelectFilter;
use Filament\Tables\Table;
use Modules\Sessions\Enums\SessionStatus;
use Modules\Sessions\Enums\SessionType;

class SessionsTable
{
    public static function configure(Table $table): Table
    {
        return $table
            ->columns([
                TextColumn::make('group.name_en')
                    ->label('Group')
                    ->description(fn ($record) => "Session #{$record->session_number}")
                    ->searchable()
                    ->sortable(),
                TextColumn::make('instructor.display_name')
                    ->label('Instructor')
                    ->icon('heroicon-o-user')
                    ->searchable()
                    ->sortable(),
                TextColumn::make('session_type')
                    ->badge()
                    ->color(fn (SessionType $state): string => match ($state) {
                        SessionType::Free => 'success',
                        SessionType::Paid => 'warning',
                    }),
                TextColumn::make('status')
                    ->badge()
                    ->color(fn (SessionStatus $state): string => match ($state) {
                        SessionStatus::Scheduled => 'info',
                        SessionStatus::Live => 'warning',
                        SessionStatus::Finished => 'success',
                        SessionStatus::Cancelled => 'danger',
                    }),
                TextColumn::make('scheduled_at')
                    ->label('Scheduled')
                    ->dateTime('M d, Y H:i')
                    ->description(fn ($record) => $record->duration_minutes ? "{$record->duration_minutes} min" : null)
                    ->sortable(),
                TextColumn::make('jitsi_room_name')
                    ->label('Room')
                    ->toggleable(isToggledHiddenByDefault: true),
            ])
            ->filters([
                SelectFilter::make('status')
                    ->options(SessionStatus::class),
                SelectFilter::make('session_type')
                    ->options(SessionType::class),
                SelectFilter::make('group')
                    ->relationship('group', 'name_en')
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
