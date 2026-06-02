<?php

namespace Database\Seeders;

use Illuminate\Database\Seeder;
use Illuminate\Support\Facades\DB;

class DefaultSettingsSeeder extends Seeder
{
    public function run(): void
    {
        $defaults = [
            ['group' => 'general', 'name' => 'session_price', 'locked' => false, 'payload' => 1200],
            ['group' => 'general', 'name' => 'group_min_members', 'locked' => false, 'payload' => 7],
            ['group' => 'general', 'name' => 'group_max_members', 'locked' => false, 'payload' => 15],
            ['group' => 'general', 'name' => 'session_duration_minutes', 'locked' => false, 'payload' => 15],
            ['group' => 'general', 'name' => 'booking_cutoff_minutes', 'locked' => false, 'payload' => 15],
            ['group' => 'general', 'name' => 'session_start_hour', 'locked' => false, 'payload' => 9],
            ['group' => 'general', 'name' => 'session_days', 'locked' => false, 'payload' => ['sunday', 'monday', 'tuesday', 'wednesday']],
            ['group' => 'general', 'name' => 'auto_start_timeout_minutes', 'locked' => false, 'payload' => 1440],
        ];

        foreach ($defaults as $default) {
            DB::table('settings')->updateOrInsert(
                ['group' => $default['group'], 'name' => $default['name']],
                [
                    'locked' => $default['locked'],
                    'payload' => json_encode($default['payload']),
                    'created_at' => now(),
                    'updated_at' => now(),
                ]
            );
        }
    }
}
