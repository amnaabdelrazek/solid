<?php

namespace App\Settings;

use Spatie\LaravelSettings\Settings;

class GeneralSettings extends Settings
{
    public int $session_price;

    public int $group_min_members;

    public int $group_max_members;

    public int $session_duration_minutes;

    public int $booking_cutoff_minutes;

    public int $session_start_hour;

    public array $session_days;

    public int $auto_start_timeout_minutes;

    public static function group(): string
    {
        return 'general';
    }
}
