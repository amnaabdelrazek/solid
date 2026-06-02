<?php

namespace App\Services\Sms;

use App\Support\Contracts\SmsContract;
use Illuminate\Support\Facades\Log;
use Twilio\Exceptions\TwilioException;
use Twilio\Rest\Client;

class TwilioSmsProvider implements SmsContract
{
    public function __construct(
        private readonly string $sid,
        private readonly string $authToken,
        private readonly string $from,
    ) {}

    public function send(string $to, string $message): bool
    {
        try {
            $client = new Client($this->sid, $this->authToken);

            $client->messages->create($to, [
                'from' => $this->from,
                'body' => $message,
            ]);

            return true;
        } catch (TwilioException $e) {
            Log::warning("Twilio SMS failed to {$to}: {$e->getMessage()}");

            return false;
        }
    }
}
