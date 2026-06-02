<?php

namespace App\Settings;

use Spatie\LaravelSettings\Settings;

class ContentSettings extends Settings
{
    public string $privacy_policy;

    public string $terms_and_conditions;

    public static function group(): string
    {
        return 'content';
    }
}
