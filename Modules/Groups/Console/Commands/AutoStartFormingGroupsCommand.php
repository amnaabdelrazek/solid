<?php

namespace Modules\Groups\Console\Commands;

use App\Settings\GeneralSettings;
use Illuminate\Console\Command;
use Modules\Groups\Enums\GroupStatus;
use Modules\Groups\Events\GroupReadyEvent;
use Modules\Groups\Models\Group;

class AutoStartFormingGroupsCommand extends Command
{
    protected $signature = 'groups:auto-start';

    protected $description = 'Auto-start forming groups that have passed the timeout period';

    public function handle(): int
    {
        $settings = app(GeneralSettings::class);
        $cutoff = now()->subMinutes($settings->auto_start_timeout_minutes);

        $formingGroups = Group::where('status', GroupStatus::Forming)
            ->where('created_at', '<=', $cutoff)
            ->get();

        if ($formingGroups->isEmpty()) {
            $this->info('No forming groups need to be auto-started.');

            return self::SUCCESS;
        }

        foreach ($formingGroups as $group) {
            $this->info("Auto-starting group {$group->id} ({$group->name_en}) with {$group->member_count} members...");
            GroupReadyEvent::dispatch($group);
        }

        $this->info("Auto-started {$formingGroups->count()} group(s).");

        return self::SUCCESS;
    }
}
