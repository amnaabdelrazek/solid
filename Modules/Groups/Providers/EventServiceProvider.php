<?php

namespace Modules\Groups\Providers;

use Illuminate\Foundation\Support\Providers\EventServiceProvider as ServiceProvider;
use Modules\Groups\Events\GroupReadyEvent;
use Modules\Groups\Listeners\CreateDefaultSessionsWhenGroupReadyListener;
use Modules\Groups\Listeners\NotifyAdminGroupReadyListener;

class EventServiceProvider extends ServiceProvider
{
    protected $listen = [
        GroupReadyEvent::class => [
            NotifyAdminGroupReadyListener::class,
            CreateDefaultSessionsWhenGroupReadyListener::class,
        ],
    ];
}
