<?php

use Spatie\LaravelSettings\Migrations\SettingsMigration;

return new class extends SettingsMigration
{
    public function up(): void
    {
        $this->migrator->add('general.session_price', 1200);
        $this->migrator->add('general.group_min_members', 7);
        $this->migrator->add('general.group_max_members', 15);
        $this->migrator->add('general.session_duration_minutes', 15);
        $this->migrator->add('general.booking_cutoff_minutes', 15);
        $this->migrator->add('general.session_start_hour', 9);
        $this->migrator->add('general.session_days', ['sunday', 'monday', 'tuesday', 'wednesday']);
        $this->migrator->add('general.auto_start_timeout_minutes', 1440);
    }
};
