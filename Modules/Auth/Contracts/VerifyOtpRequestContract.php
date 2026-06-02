<?php

namespace Modules\Auth\Contracts;

interface VerifyOtpRequestContract
{
    public function getOtp(): string;

    public function getToken(): ?string;
}
