<?php

namespace Modules\Payments\Factories;

use InvalidArgumentException;
use Modules\Payments\Contracts\PaymentGatewayFactoryInterface;
use Modules\Payments\Contracts\PaymentGatewayInterface;
use Modules\Payments\Gateways\FawryGateway;
use Modules\Payments\Gateways\MasaryGateway;
use Modules\Payments\Gateways\PaymobGateway;

class EgyptianPaymentGatewayFactory implements PaymentGatewayFactoryInterface
{
    public function make(?string $gateway = null): PaymentGatewayInterface
    {
        $gatewayName = strtolower($gateway ?? config('payments.default_gateway', 'paymob'));

        return match ($gatewayName) {
            'paymob' => app(PaymobGateway::class),
            'fawry' => app(FawryGateway::class),
            'masary' => app(MasaryGateway::class),
            default => throw new InvalidArgumentException("Unsupported payment gateway [{$gatewayName}]."),
        };
    }
}
