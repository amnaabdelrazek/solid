<?php

namespace App\Filament\Resources\Sessions\Tables;

use Filament\Actions\BulkActionGroup;
use Filament\Actions\DeleteBulkAction;
use Filament\Actions\EditAction;
use Filament\Tables\Columns\IconColumn;
use Filament\Tables\Columns\TextColumn;
use Filament\Tables\Filters\SelectFilter;
use Filament\Tables\Table;

class BreakoutRoomsTable
{
    public static function configure(Table $table): Table
    {
        return $table
            ->columns([
                TextColumn::make('room_name')
                    ->searchable()
                    ->sortable(),
                TextColumn::make('session.group.name_en')
                    ->label('Session')
                    ->description(fn ($record) => "Session #{$record->session_id}")
                    ->searchable()
                    ->sortable(),
                TextColumn::make('creator.display_name')
                    ->label('Created By')
                    ->icon('heroicon-o-user')
                    ->searchable(),
                IconColumn::make('is_open')
                    ->label('Open')
                    ->boolean()
                    ->sortable(),
                TextColumn::make('members_count')
                    ->counts('members')
                    ->label('Members')
                    ->badge()
                    ->color('info'),
                TextColumn::make('created_at')
                    ->dateTime()
                    ->sortable()
                    ->toggleable(isToggledHiddenByDefault: true),
            ])
            ->filters([
                SelectFilter::make('session_id')
                    ->label('Session')
                    ->relationship('session', 'session_number')
                    ->searchable()
                    ->preload(),
                SelectFilter::make('is_open')
                    ->label('Status')
                    ->options([
                        '1' => 'Open',
                        '0' => 'Closed',
                    ]),
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
