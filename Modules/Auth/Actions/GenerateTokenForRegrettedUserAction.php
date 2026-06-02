<?php

namespace Modules\Auth\Actions;

use App\Support\Traits\MakeAble;
use Illuminate\Support\Facades\Hash;
use Modules\User\Models\User;

final class GenerateTokenForRegrettedUserAction
{
    use MakeAble;

    public function execute(User $user): string
    {
        return $user->id.'|'.Hash::make(now()->timestamp.'|'.$user->id);
    }
}
