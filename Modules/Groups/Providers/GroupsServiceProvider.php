<?php

namespace Modules\Groups\Providers;

use Illuminate\Console\Scheduling\Schedule;
use Modules\Groups\Console\Commands\AutoStartFormingGroupsCommand;
use Nwidart\Modules\Support\ModuleServiceProvider;

class GroupsServiceProvider extends ModuleServiceProvider
{
    /**
     * The name of the module.
     */
    protected string $name = 'Groups';

    /**
     * The lowercase version of the name of the module.
     */
    protected string $nameLower = 'groups';

    /**
     * Command classes to register.
     *
     * @var string[]
     */
    protected array $commands = [
        AutoStartFormingGroupsCommand::class,
    ];

    /**
     * Provider classes to register.
     *
     * @var string[]
     */
    protected array $providers = [
        EventServiceProvider::class,
        RouteServiceProvider::class,
    ];

    /**
     * Define module schedules.
     */
    protected function configureSchedules(Schedule $schedule): void
    {
        $schedule->command('groups:auto-start')->hourly();
    }
}
