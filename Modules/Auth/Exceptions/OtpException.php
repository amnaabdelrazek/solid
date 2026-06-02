<?php

namespace Modules\Auth\Exceptions;

use Exception;

class OtpException extends Exception
{
    public static function expired(): static
    {
        return new static(trans('messages.otp.expired'));
    }

    public static function invalid(): static
    {
        return new static(trans('messages.otp.invalid'));
    }
}
