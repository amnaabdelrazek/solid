<?php

namespace Modules\Payments\Repositories;

use Modules\Payments\Enums\PaymentStatus;
use Modules\Payments\Models\Payment;

final class EloquentPaymentRepository implements PaymentRepositoryInterface
{
    public function hasPaidForSession(int $userId, int $sessionId): bool
    {
        return Payment::where('user_id', $userId)
            ->where('session_id', $sessionId)
            ->where('status', PaymentStatus::Paid)
            ->exists();
    }

    public function findByTransactionId(string $transactionId): ?Payment
    {
        return Payment::where('gateway_transaction_id', $transactionId)->first();
    }

    public function create(array $data): Payment
    {
        return Payment::create($data);
    }

    public function update(Payment $payment, array $data): void
    {
        $payment->update($data);
    }
}
