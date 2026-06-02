<?php

namespace App\Providers;

use App\Services\Sms\TwilioSmsProvider;
use App\Support\Contracts\SmsContract;
use App\Support\Repositories\RepositoryBinder;
use Illuminate\Support\ServiceProvider;

class AppServiceProvider extends ServiceProvider
{
    /**
     * @throws \Exception
     */
    public function register(): void
    {
        (new RepositoryBinder)->bind($this->app);

        $this->app->singleton(SmsContract::class, fn () => new TwilioSmsProvider(
            config('services.twilio.sid'),
            config('services.twilio.auth_token'),
            config('services.twilio.from'),
        ));
    }

    public function boot(): void
    {
        //
    }
}
