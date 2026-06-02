<?php

namespace Modules\Payments\Repositories;

use Modules\Payments\Models\Payment;

interface PaymentRepositoryInterface
{
    public function hasPaidForSession(int $userId, int $sessionId): bool;

    public function findByTransactionId(string $transactionId): ?Payment;

    public function create(array $data): Payment;

    public function update(Payment $payment, array $data): void;
}
