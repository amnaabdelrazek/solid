<?php

namespace Modules\Auth\Actions;

use App\Support\Traits\MakeAble;

class GenerateOtpAction
{
    use MakeAble;

    public function execute(): mixed
    {
        return config('otp.fixed_otp');
    }
}
