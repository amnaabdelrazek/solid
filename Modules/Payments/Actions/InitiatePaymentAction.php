<?php

namespace Modules\Payments\Actions;

use App\Support\Traits\MakeAble;
use Modules\Payments\Services\PaymentService;
use Modules\Sessions\Models\Session;
use Modules\User\Models\User;

final class InitiatePaymentAction
{
    use MakeAble;

    public function __construct(
        private readonly PaymentService $paymentService,
    ) {}

    public function execute(User $user, Session $session): array
    {
        return $this->paymentService->initiate($user, $session);
    }
}
