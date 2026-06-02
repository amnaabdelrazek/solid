<?php

namespace Modules\Payments\Contracts;

interface PaymentGatewayFactoryInterface
{
    public function make(?string $gateway = null): PaymentGatewayInterface;
}
