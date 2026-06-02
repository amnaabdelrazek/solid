<?php

namespace Modules\Sessions\Providers;

use Illuminate\Foundation\Support\Providers\EventServiceProvider as ServiceProvider;
use Modules\Sessions\Events\SessionEndedEvent;
use Modules\Sessions\Listeners\SendPostSessionRecommendationsListener;

class EventServiceProvider extends ServiceProvider
{
    protected $listen = [
        SessionEndedEvent::class => [
            SendPostSessionRecommendationsListener::class,
        ],
    ];
}
