<?php

namespace Modules\Payments\Providers;

use Illuminate\Console\Scheduling\Schedule;
use Modules\Payments\Contracts\PaymentGatewayFactoryInterface;
use Modules\Payments\Contracts\PaymentGatewayInterface;
use Modules\Payments\Factories\EgyptianPaymentGatewayFactory;
use Nwidart\Modules\Support\ModuleServiceProvider;

class PaymentsServiceProvider extends ModuleServiceProvider
{
    public function register(): void
    {
        parent::register();

        $this->app->singleton(PaymentGatewayFactoryInterface::class, EgyptianPaymentGatewayFactory::class);

        $this->app->bind(PaymentGatewayInterface::class, function ($app) {
            $factory = $app->make(PaymentGatewayFactoryInterface::class);

            return $factory->make();
        });
    }

    /**
     * The name of the module.
     */
    protected string $name = 'Payments';

    /**
     * The lowercase version of the module name.
     */
    protected string $nameLower = 'payments';

    /**
     * Command classes to register.
     *
     * @var string[]
     */
    // protected array $commands = [];

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
     *
     * @param  $schedule
     */
    // protected function configureSchedules(Schedule $schedule): void
    // {
    //     $schedule->command('inspire')->hourly();
    // }
}
