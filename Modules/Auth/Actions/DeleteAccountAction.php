<?php

namespace Modules\Auth\Actions;

use App\Support\Traits\MakeAble;
use Modules\User\Models\User;

final class DeleteAccountAction
{
    use MakeAble;

    public function execute(User $user): void
    {
        $user->tokens()->delete();

        $user->update([
            'fcm_token' => null,
            'active_device_id' => null,
            'is_active' => false,
        ]);

        $user->delete();
    }
}
