<?php

namespace App\Filament\Resources\Groups\Schemas;

use Filament\Forms\Components\Select;
use Filament\Forms\Components\TextInput;
use Filament\Schemas\Components\Grid;
use Filament\Schemas\Components\Section;
use Filament\Schemas\Schema;
use Modules\Auth\Enums\UserRole;
use Modules\Groups\Enums\GroupStatus;
use Modules\Groups\Enums\GroupType;
use Modules\User\Models\User;

class GroupForm
{
    public static function configure(Schema $schema): Schema
    {
        return $schema
            ->components([
                Grid::make(2)
                    ->schema([
                        Section::make('General Information')
                            ->description('Basic group details and names.')
                            ->icon('heroicon-o-information-circle')
                            ->schema([
                                Grid::make(2)
                                    ->schema([
                                        TextInput::make('name_ar')
                                            ->label('Name (Arabic)')
                                            ->required()
                                            ->maxLength(255),
                                        TextInput::make('name_en')
                                            ->label('Name (English)')
                                            ->required()
                                            ->maxLength(255),
                                    ]),
                                Select::make('instructor_id')
                                    ->relationship('instructor', 'display_name')
                                    ->options(User::where('role', UserRole::Instructor->value)->pluck('display_name', 'id'))
                                    ->searchable()
                                    ->preload()
                                    ->required()
                                    ->prefixIcon('heroicon-o-user-badge'),
                                Select::make('substance_category_id')
                                    ->relationship('substanceCategory', 'name_en')
                                    ->searchable()
                                    ->preload()
                                    ->nullable(),
                            ])
                            ->columnSpan(1),

                        Section::make('Configuration')
                            ->description('Group status, type and member limits.')
                            ->icon('heroicon-o-cog-6-tooth')
                            ->schema([
                                Grid::make(2)
                                    ->schema([
                                        Select::make('group_type')
                                            ->options(GroupType::class)
                                            ->required()
                                            ->native(false),
                                        Select::make('status')
                                            ->options(GroupStatus::class)
                                            ->required()
                                            ->native(false),
                                    ]),
                                Grid::make(2)
                                    ->schema([
                                        TextInput::make('min_members')
                                            ->numeric()
                                            ->default(1)
                                            ->minValue(1)
                                            ->required()
                                            ->prefixIcon('heroicon-o-users'),
                                        TextInput::make('max_members')
                                            ->numeric()
                                            ->default(10)
                                            ->minValue(1)
                                            ->required()
                                            ->prefixIcon('heroicon-o-user-plus'),
                                    ]),
                            ])
                            ->columnSpan(1),
                    ]),
            ]);
    }
}
