<?php

namespace App\Exceptions\Sessions;

use Symfony\Component\HttpKernel\Exception\HttpException;

class PaymentRequiredException extends HttpException
{
    public function __construct()
    {
        parent::__construct(402, 'Payment is required to join this session.');
    }
}
