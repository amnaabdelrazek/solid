<?php

namespace App\Filament\Pages\Settings;

use App\Settings\ContentSettings;
use Filament\Actions\Action;
use Filament\Forms\Components\RichEditor;
use Filament\Forms\Concerns\InteractsWithForms;
use Filament\Forms\Contracts\HasForms;
use Filament\Notifications\Notification;
use Filament\Pages\Page;
use Filament\Schemas\Components\Actions;
use Filament\Schemas\Components\EmbeddedSchema;
use Filament\Schemas\Components\Form;
use Filament\Schemas\Schema;

class ContentSettingsPage extends Page implements HasForms
{
    use InteractsWithForms;

    protected static string|null|\BackedEnum $navigationIcon = 'heroicon-o-document-text';

    protected static string|null|\UnitEnum $navigationGroup = 'Settings';

    protected static ?string $title = 'Content Settings';

    protected static ?string $slug = 'content-settings';

    public ?array $data = [];

    public function mount(): void
    {
        $this->form->fill($this->getSettingsData());
    }

    protected static function getSettingsData(): array
    {
        $settings = app(ContentSettings::class);

        return [
            'privacy_policy' => $settings->privacy_policy,
            'terms_and_conditions' => $settings->terms_and_conditions,
        ];
    }

    public function form(Schema $form): Schema
    {
        return $form
            ->components([
                RichEditor::make('privacy_policy')
                    ->label('Privacy Policy')
                    ->toolbarButtons([
                        'bold',
                        'bulletList',
                        'italic',
                        'link',
                        'redo',
                        'strike',
                        'undo',
                    ])
                    ->columnSpanFull(),
                RichEditor::make('terms_and_conditions')
                    ->label('Terms and Conditions')
                    ->toolbarButtons([
                        'bold',
                        'bulletList',
                        'italic',
                        'link',
                        'redo',
                        'strike',
                        'undo',
                    ])
                    ->columnSpanFull(),
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

        $settings = app(ContentSettings::class);
        $settings->privacy_policy = $data['privacy_policy'] ?? '';
        $settings->terms_and_conditions = $data['terms_and_conditions'] ?? '';
        $settings->save();

        Notification::make()
            ->title('Content settings saved successfully.')
            ->success()
            ->send();
    }
}
