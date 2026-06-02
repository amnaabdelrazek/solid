<?php

namespace App\Support\Contracts;

interface SmsContract
{
    public function send(string $to, string $message): bool;
}
