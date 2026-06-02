<?php

namespace App\Filament\Pages\Settings;

use App\Settings\GeneralSettings;
use Filament\Actions\Action;
use Filament\Forms\Components\KeyValue;
use Filament\Forms\Components\TextInput;
use Filament\Forms\Concerns\InteractsWithForms;
use Filament\Forms\Contracts\HasForms;
use Filament\Notifications\Notification;
use Filament\Pages\Page;
use Filament\Schemas\Components\Actions;
use Filament\Schemas\Components\EmbeddedSchema;
use Filament\Schemas\Components\Form;
use Filament\Schemas\Components\Grid;
use Filament\Schemas\Components\Section;
use Filament\Schemas\Schema;

class GeneralSettingsPage extends Page implements HasForms
{
    use InteractsWithForms;

    protected static string|null|\BackedEnum $navigationIcon = 'heroicon-o-cog-6-tooth';

    protected static string|null|\UnitEnum $navigationGroup = 'Settings';

    protected static ?string $title = 'General Settings';

    protected static ?string $slug = 'general-settings';

    public ?array $data = [];

    public function mount(): void
    {
        $this->form->fill($this->getSettingsData());
    }

    protected static function getSettingsData(): array
    {
        $settings = app(GeneralSettings::class);

        return [
            'session_price' => $settings->session_price,
            'group_min_members' => $settings->group_min_members,
            'group_max_members' => $settings->group_max_members,
            'session_duration_minutes' => $settings->session_duration_minutes,
            'booking_cutoff_minutes' => $settings->booking_cutoff_minutes,
            'session_start_hour' => $settings->session_start_hour,
            'session_days' => $settings->session_days,
            'auto_start_timeout_minutes' => $settings->auto_start_timeout_minutes,
        ];
    }

    public function form(Schema $form): Schema
    {
        return $form
            ->components([
                Grid::make(2)
                    ->schema([
                        Section::make('Session Configuration')
                            ->description('Pricing and duration settings.')
                            ->icon('heroicon-o-presentation-chart-bar')
                            ->schema([
                                TextInput::make('session_price')
                                    ->numeric()
                                    ->prefix('$')
                                    ->required()
                                    ->minValue(0),
                                TextInput::make('session_duration_minutes')
                                    ->numeric()
                                    ->suffix('min')
                                    ->required()
                                    ->minValue(15),
                                TextInput::make('booking_cutoff_minutes')
                                    ->label('Booking Cutoff')
                                    ->numeric()
                                    ->suffix('min before')
                                    ->required()
                                    ->minValue(0),
                                TextInput::make('session_start_hour')
                                    ->label('Start Hour')
                                    ->numeric()
                                    ->suffix(':00 (24h)')
                                    ->minValue(0)
                                    ->maxValue(23)
                                    ->required(),
                            ])
                            ->columnSpan(1),

                        Section::make('Group Configuration')
                            ->description('Group size limits and auto-start settings.')
                            ->icon('heroicon-o-users')
                            ->schema([
                                TextInput::make('group_min_members')
                                    ->numeric()
                                    ->required()
                                    ->minValue(1),
                                TextInput::make('group_max_members')
                                    ->numeric()
                                    ->required()
                                    ->minValue(1),
                                TextInput::make('auto_start_timeout_minutes')
                                    ->label('Auto-Start Timeout')
                                    ->numeric()
                                    ->suffix('min')
                                    ->required()
                                    ->minValue(0),
                                KeyValue::make('session_days')
                                    ->label('Session Days')
                                    ->keyLabel('Key')
                                    ->valueLabel('Day Name')
                                    ->addActionLabel('Add Day'),
                            ])
                            ->columnSpan(1),
                    ]),
            ])
            ->statePath('data');
    }

    public function content(Schema $schema): Schema
    {
        return $schema
            ->components([
                Form::make([EmbeddedSchema::make('form')])
                    ->id('form')
                    ->livewireSubmitHandler('save')
                    ->footer([
                        Actions::make([
                            Action::make('save')
                                ->label('Save')
                                ->action('save'),
                        ])
                            ->alignment('end'),
                    ]),
            ]);
    }

    public function save(): void
    {
        $data = $this->form->getState();

        $settings = app(GeneralSettings::class);
        $settings->session_price = $data['session_price'];
        $settings->group_min_members = $data['group_min_members'];
        $settings->group_max_members = $data['group_max_members'];
        $settings->session_duration_minutes = $data['session_duration_minutes'];
        $settings->booking_cutoff_minutes = $data['booking_cutoff_minutes'];
        $settings->session_start_hour = $data['session_start_hour'];
        $settings->session_days = $data['session_days'] ?? [];
        $settings->auto_start_timeout_minutes = $data['auto_start_timeout_minutes'];
        $settings->save();

        Notification::make()
            ->title('General settings saved successfully.')
            ->success()
            ->send();
    }
}
