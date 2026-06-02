<?php

namespace App\Filament\Resources\Groups\Tables;

use Filament\Actions\BulkActionGroup;
use Filament\Actions\DeleteBulkAction;
use Filament\Actions\EditAction;
use Filament\Tables\Columns\TextColumn;
use Filament\Tables\Filters\SelectFilter;
use Filament\Tables\Table;
use Modules\Groups\Enums\GroupStatus;
use Modules\Groups\Enums\GroupType;

class GroupsTable
{
    public static function configure(Table $table): Table
    {
        return $table
            ->columns([
                TextColumn::make('name_en')
                    ->label('Group Name')
                    ->description(fn ($record) => $record->name_ar)
                    ->searchable()
                    ->sortable(),
                TextColumn::make('instructor.display_name')
                    ->label('Instructor')
                    ->icon('heroicon-o-user')
                    ->searchable()
                    ->sortable(),
                TextColumn::make('group_type')
                    ->badge()
                    ->color(fn (GroupType $state): string => match ($state) {
                        GroupType::SingleCategory => 'success',
                        GroupType::Mixed => 'warning',
                    })
                    ->searchable(),
                TextColumn::make('status')
                    ->badge()
                    ->color(fn (GroupStatus $state): string => match ($state) {
                        GroupStatus::Active => 'success',
                        GroupStatus::Forming => 'info',
                        GroupStatus::Completed => 'gray',
                        GroupStatus::Dissolved => 'danger',
                    })
                    ->searchable(),
                TextColumn::make('members_count')
                    ->counts('members')
                    ->label('Members')
                    ->badge()
                    ->color(fn ($state, $record) => $state >= $record->max_members ? 'danger' : ($state >= $record->min_members ? 'success' : 'warning')),
                TextColumn::make('limits')
                    ->label('Limits (Min/Max)')
                    ->getStateUsing(fn ($record) => "{$record->min_members} / {$record->max_members}")
                    ->alignCenter(),
                TextColumn::make('created_at')
                    ->dateTime()
                    ->sortable()
                    ->toggleable(isToggledHiddenByDefault: true),
            ])
            ->filters([
                SelectFilter::make('group_type')
                    ->options(GroupType::class),
                SelectFilter::make('status')
                    ->options(GroupStatus::class),
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
