<?php

namespace Modules\Payments\Services;

use App\Settings\GeneralSettings;
use App\Support\Traits\MakeAble;
use Illuminate\Validation\ValidationException;
use Modules\Groups\Services\GroupMatchingService;
use Modules\Payments\Contracts\PaymentGatewayFactoryInterface;
use Modules\Payments\DTOs\PaymentDTO;
use Modules\Payments\DTOs\PaymentVerifyResult;
use Modules\Payments\Enums\PaymentStatus;
use Modules\Payments\Models\Payment;
use Modules\Payments\Repositories\PaymentRepositoryInterface;
use Modules\Sessions\Models\Session;
use Modules\Sessions\Models\SessionAttendance;
use Modules\User\Models\User;

final class PaymentService
{
    use MakeAble;

    public function __construct(
        private readonly PaymentRepositoryInterface $payments,
        private readonly PaymentGatewayFactoryInterface $gatewayFactory,
        private readonly GroupMatchingService $groupMatchingService,
    ) {}

    public function initiate(User $user, Session $session): array
    {
        $this->assertSessionCanBeBooked($user, $session);

        $price = (float) data_get($session->session_metadata, 'price', app(GeneralSettings::class)->session_price);
        $dto = new PaymentDTO($user, $session, $price);
        $gateway = $this->gatewayFactory->make();
        $result = $gateway->initiate($dto);

        $payment = $this->payments->create([
            'user_id' => $user->id,
            'session_id' => $session->id,
            'amount' => $dto->amount,
            'currency' => $dto->currency,
            'status' => PaymentStatus::Pending,
            'gateway' => config('payments.gateway'),
            'gateway_transaction_id' => $result->transactionId,
        ]);

        return ['payment_url' => $result->paymentUrl, 'payment' => $payment];
    }

    public function processWebhook(string $gateway, array $payload): void
    {
        $gatewayService = $this->gatewayFactory->make($gateway);
        $result = $gatewayService->verify($payload);
        $payment = $this->payments->findByTransactionId($result->transactionId);

        if (! $payment || $payment->status === PaymentStatus::Paid) {
            return;
        }

        $this->updatePayment($payment, $result);

        if ($result->status === PaymentStatus::Paid) {
            $this->groupMatchingService->match($payment->user);
        }
    }

    private function updatePayment(Payment $payment, PaymentVerifyResult $result): void
    {
        $this->payments->update($payment, [
            'status' => $result->status,
            'gateway_response' => $result->rawPayload,
            'paid_at' => $result->status === PaymentStatus::Paid ? now() : null,
        ]);
    }

    private function assertSessionCanBeBooked(User $user, Session $session): void
    {
        $settings = app(GeneralSettings::class);

        if ($session->scheduled_at->lte(now()->addMinutes($settings->booking_cutoff_minutes))) {
            throw ValidationException::withMessages(['session_id' => "Booking must be at least {$settings->booking_cutoff_minutes} minutes before session time."]);
        }

        if ($session->session_type->value !== 'paid') {
            throw ValidationException::withMessages(['session_id' => 'All sessions must be paid.']);
        }

        $group = $user->groups()->latest('group_members.joined_at')->first();

        if (! $group || (int) $group->id !== (int) $session->group_id) {
            throw ValidationException::withMessages(['session_id' => 'User can only book sessions in their group.']);
        }

        $lastAttendedSessionNumber = SessionAttendance::query()
            ->where('user_id', $user->id)
            ->whereHas('session', fn ($q) => $q->where('group_id', $group->id))
            ->where('was_present', true)
            ->join('therapy_sessions', 'session_attendances.session_id', '=', 'therapy_sessions.id')
            ->max('therapy_sessions.session_number') ?? 0;

        if ($session->session_number > $lastAttendedSessionNumber) {
            throw ValidationException::withMessages(['session_id' => 'You must book sessions in order.']);
        }
    }
}
