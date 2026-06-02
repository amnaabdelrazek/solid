<?php

namespace Modules\Auth\Listeners;

use App\Support\Contracts\SmsContract;
use Modules\Auth\Actions\GenerateOtpAction;
use Modules\Auth\Events\PasswordResetRequested;

class SendPasswordResetOtp
{
    public function __construct(
        private readonly SmsContract $sms,
    ) {}

    public function handle(PasswordResetRequested $event): void
    {
        $otp = GenerateOtpAction::make()->execute();
        $ttl = config('otp.ttl', 300);

        cache()->put("password_reset_otp:{$event->user->id}", $otp, $ttl);

        if ($event->user->mobile_number) {
            $this->sms->send(
                $event->user->mobile_number,
                __('messages.otp.sms_body', ['otp' => $otp]),
            );
        }
    }
}
