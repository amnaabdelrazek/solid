<?php

namespace App\Filament\Resources\Sessions\Tables;

use Filament\Actions\BulkActionGroup;
use Filament\Actions\DeleteBulkAction;
use Filament\Actions\EditAction;
use Filament\Tables\Columns\IconColumn;
use Filament\Tables\Columns\TextColumn;
use Filament\Tables\Filters\SelectFilter;
use Filament\Tables\Table;

class SessionAttendancesTable
{
    public static function configure(Table $table): Table
    {
        return $table
            ->columns([
                TextColumn::make('session.group.name_en')
                    ->label('Session')
                    ->description(fn ($record) => "Session #{$record->session_id}")
                    ->searchable()
                    ->sortable(),
                TextColumn::make('user.display_name')
                    ->label('User')
                    ->icon('heroicon-o-user')
                    ->searchable()
                    ->sortable(),
                IconColumn::make('was_present')
                    ->label('Present')
                    ->boolean()
                    ->sortable(),
                TextColumn::make('rating')
                    ->badge()
                    ->color(fn ($state) => match (true) {
                        $state >= 4 => 'success',
                        $state >= 3 => 'warning',
                        default => 'danger',
                    })
                    ->toggleable(),
                TextColumn::make('joined_at')
                    ->dateTime('M d, Y H:i')
                    ->sortable()
                    ->toggleable(),
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
                SelectFilter::make('user_id')
                    ->label('User')
                    ->relationship('user', 'display_name')
                    ->searchable()
                    ->preload(),
                SelectFilter::make('was_present')
                    ->label('Status')
                    ->options([
                        '1' => 'Present',
                        '0' => 'Absent',
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
