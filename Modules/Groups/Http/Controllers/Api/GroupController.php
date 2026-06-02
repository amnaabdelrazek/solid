<?php

namespace Modules\Groups\Http\Controllers\Api;

use App\Http\Controllers\Api\ApiController;
use App\Settings\GeneralSettings;
use Illuminate\Http\JsonResponse;
use Illuminate\Http\Request;
use Modules\Groups\Http\Resources\GroupResource;
use Modules\Groups\Repositories\GroupRepositoryInterface;
use Modules\Groups\Services\GroupMatchingService;
use Modules\Payments\Actions\InitiatePaymentAction;
use Modules\Sessions\Models\Session;

class GroupController extends ApiController
{
    public function index(GroupRepositoryInterface $groupRepository)
    {
        return $this->apiBody([
            'groups' => GroupResource::collection($groupRepository->list()),
        ])->apiResponse();
    }

    public function myGroup(Request $request): JsonResponse
    {
        $group = $request->user()
            ->groups()
            ->with(['instructor', 'members'])
            ->latest('pivot_joined_at')
            ->firstOrFail();

        return $this->apiBody([
            'group' => GroupResource::make($group),
        ])->apiResponse();
    }

    public function subscribe(
        Request $request,
        GroupMatchingService $matchingService,
        InitiatePaymentAction $paymentAction,
        GroupRepositoryInterface $groupRepository
    ): JsonResponse {
        $user = $request->user();

        // 1. Check if already in a group
        if ($user->groups()->exists()) {
            return $this->apiMessage('You are already subscribed to a group.')->apiResponse(422);
        }

        // 2. Find/Check group capacity (Match without adding yet)
        // For simplicity, we'll try to find an open group to check capacity
        $categoryIds = $user->substances()->pluck('substance_id')->toArray(); // Simplified
        // Actually, matching service does a lot. Let's just use it but carefully.

        // Let's assume we want to initiate payment for the "next" session that would be created
        // or just a general price from settings.
        $settings = app(GeneralSettings::class);

        // To keep it simple and consistent with existing PaymentAction (which requires a Session),
        // we'll find or create a group, and if it's ready, it has sessions.
        // If not ready, we might need a "Subscription" session or just a flat fee.

        // IMPROVEMENT: Let's create a temporary session or use a placeholder if needed,
        // but the current InitiatePaymentAction strictly requires a Session model.

        $group = $matchingService->match($user); // This joins the user.

        // If the group has sessions (already ready), initiate payment for the first one.
        $session = $group->sessions()->orderBy('session_number')->first();

        if ($session) {
            $paymentResult = $paymentAction->execute($user, $session);

            return $this->apiBody([
                'group' => GroupResource::make($group),
                'payment' => $paymentResult['payment'],
                'payment_url' => $paymentResult['payment_url'],
            ])->apiMessage('Subscription initiated. Please complete payment.')->apiResponse();
        }

        return $this->apiBody([
            'group' => GroupResource::make($group),
        ])->apiMessage('Joined group successfully. Waiting for other members to start sessions.')->apiResponse();
    }
}
