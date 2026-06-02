<?php

namespace Modules\Auth\Providers;

use Illuminate\Foundation\Support\Providers\EventServiceProvider as ServiceProvider;
use Modules\Auth\Events\PasswordResetRequested;
use Modules\Auth\Events\UserRegistered;
use Modules\Auth\Listeners\SendOtpForRegisteredUser;
use Modules\Auth\Listeners\SendPasswordResetOtp;

class EventServiceProvider extends ServiceProvider
{
    protected $listen = [
        UserRegistered::class => [
            SendOtpForRegisteredUser::class,
        ],
        PasswordResetRequested::class => [
            SendPasswordResetOtp::class,
        ],
    ];
}
