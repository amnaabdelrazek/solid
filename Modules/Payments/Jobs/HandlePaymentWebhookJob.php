<?php

namespace Modules\Payments\Jobs;

use Illuminate\Contracts\Queue\ShouldQueue;
use Illuminate\Foundation\Queue\Queueable;
use Illuminate\Support\Facades\DB;
use Modules\Notifications\Jobs\AdminAlertJob;
use Modules\Payments\Services\PaymentService;
use Throwable;

class HandlePaymentWebhookJob implements ShouldQueue
{
    use Queueable;

    public int $tries = 5;

    public int $backoff = 60;

    public int $timeout = 30;

    public bool $failOnTimeout = true;

    public function __construct(
        public readonly string $gateway,
        public readonly array $payload,
    ) {}

    public function handle(PaymentService $service): void
    {
        DB::transaction(fn () => $service->processWebhook($this->gateway, $this->payload));
    }

    public function failed(Throwable $e): void
    {
        AdminAlertJob::dispatch('Payment webhook failed', $this->payload, $e->getMessage());
    }
}
