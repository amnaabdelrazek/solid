<?php

namespace App\Filament\Resources\Users;

use App\Filament\Resources\Users\Pages\CreateAddictionProfile;
use App\Filament\Resources\Users\Pages\EditAddictionProfile;
use App\Filament\Resources\Users\Pages\ListAddictionProfiles;
use App\Filament\Resources\Users\Schemas\AddictionProfileForm;
use App\Filament\Resources\Users\Tables\AddictionProfilesTable;
use BackedEnum;
use Filament\Resources\Resource;
use Filament\Schemas\Schema;
use Filament\Support\Icons\Heroicon;
use Filament\Tables\Table;
use Modules\User\Models\AddictionProfile;

class AddictionProfileResource extends Resource
{
    protected static ?string $model = AddictionProfile::class;

    protected static string|BackedEnum|null $navigationIcon = Heroicon::OutlinedHeart;

    protected static ?string $recordTitleAttribute = 'id';

    protected static string|\UnitEnum|null $navigationGroup = 'Users';

    public static function form(Schema $schema): Schema
    {
        return AddictionProfileForm::configure($schema);
    }

    public static function table(Table $table): Table
    {
        return AddictionProfilesTable::configure($table);
    }

    public static function getRelations(): array
    {
        return [
            //
        ];
    }

    public static function getPages(): array
    {
        return [
            'index' => ListAddictionProfiles::route('/'),
            'create' => CreateAddictionProfile::route('/create'),
            'edit' => EditAddictionProfile::route('/{record}/edit'),
        ];
    }
}
