<?php

namespace App\Filament\Resources\Users\Schemas;

use Filament\Forms\Components\Select;
use Filament\Forms\Components\SpatieMediaLibraryFileUpload;
use Filament\Forms\Components\TagsInput;
use Filament\Forms\Components\Textarea;
use Filament\Forms\Components\TextInput;
use Filament\Forms\Components\Toggle;
use Filament\Schemas\Components\Grid;
use Filament\Schemas\Components\Section;
use Filament\Schemas\Schema;
use Modules\Auth\Enums\UserRole;

class UserForm
{
    public static function configure(Schema $schema): Schema
    {
        return $schema
            ->components([
                Grid::make(2)
                    ->schema([
                        Section::make('Personal Information')
                            ->description('Basic information about the user.')
                            ->icon('heroicon-o-user')
                            ->schema([
                                TextInput::make('display_name')
                                    ->required()
                                    ->maxLength(255)
                                    ->columnSpanFull(),
                                TextInput::make('mobile_number')
                                    ->tel()
                                    ->required()
                                    ->unique(ignoreRecord: true)
                                    ->maxLength(20),
                                Select::make('preferred_language')
                                    ->options([
                                        'ar' => 'العربية',
                                        'en' => 'English',
                                    ])
                                    ->required()
                                    ->default('ar')
                                    ->native(false),
                            ])
                            ->columnSpan(1),

                        Section::make('Account Settings')
                            ->description('Login credentials and role management.')
                            ->icon('heroicon-o-lock-closed')
                            ->schema([
                                TextInput::make('username')
                                    ->required()
                                    ->unique(ignoreRecord: true)
                                    ->maxLength(255),
                                TextInput::make('password')
                                    ->password()
                                    ->revealable()
                                    ->dehydrated(fn ($state) => filled($state))
                                    ->required(fn ($operation): bool => $operation === 'create')
                                    ->label('Password'),
                                Select::make('role')
                                    ->options(UserRole::class)
                                    ->required()
                                    ->native(false),
                                Toggle::make('is_active')
                                    ->required()
                                    ->default(true)
                                    ->inline(false),
                            ])
                            ->columnSpan(1),
                    ]),
                Section::make('Instructor Profile')
                    ->description('Additional details for instructor profiles.')
                    ->icon('heroicon-o-academic-cap')
                    ->schema([
                        SpatieMediaLibraryFileUpload::make('avatar')
                            ->collection('avatar')
                            ->avatar()
                            ->columnSpanFull(),
                        Textarea::make('bio')
                            ->label('About / Bio')
                            ->rows(4)
                            ->columnSpanFull(),
                        TagsInput::make('experience')
                            ->label('Experience')
                            ->placeholder('Add an experience item and press enter')
                            ->columnSpanFull(),
                        Textarea::make('quote')
                            ->label('Quote')
                            ->rows(3)
                            ->columnSpanFull(),
                    ])
                    ->collapsed(),
            ]);
    }
}
