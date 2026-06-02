<?php

namespace Modules\Notifications\Jobs;

use Illuminate\Contracts\Queue\ShouldQueue;
use Illuminate\Foundation\Queue\Queueable;
use Illuminate\Support\Facades\Http;
use Illuminate\Support\Facades\Log;

class AdminAlertJob implements ShouldQueue
{
    use Queueable;

    public int $tries = 2;

    public function __construct(
        public readonly string $subject,
        public readonly array $context = [],
        public readonly string $errorMessage = '',
    ) {}

    public function handle(): void
    {
        $webhookUrl = config('services.slack.webhook_url');

        if (! $webhookUrl) {
            Log::critical("[AdminAlert] {$this->subject}", $this->context);

            return;
        }

        Http::post($webhookUrl, [
            'text' => ":warning: *{$this->subject}*\n```{$this->errorMessage}```",
        ]);
    }
}
