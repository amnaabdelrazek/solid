<?php

namespace Modules\Auth\Actions;

use App\Support\Traits\MakeAble;
use Illuminate\Support\Facades\Hash;
use Modules\Auth\DTOs\AddictionProfileDTO;
use Modules\Auth\DTOs\RegisterUserDTO;
use Modules\Auth\Enums\UserRole;
use Modules\Groups\Jobs\MatchUserToGroupJob;
use Modules\User\Models\User;

final class RegisterUserAction
{
    use MakeAble;

    public function execute(RegisterUserDTO $dto): User
    {
        $user = User::query()->where('mobile_number', $dto->mobileNumber)->first();
        if (! $user) {
            $user = User::query()->create([
                'display_name' => $dto->displayName,
                'mobile_number' => $dto->mobileNumber,
                'password' => Hash::make($dto->password),
                'role' => UserRole::Addict,
                'preferred_language' => $dto->preferredLanguage,
                'is_active' => false,
            ]);
            app(SaveAddictionProfileAction::class)->execute($user, AddictionProfileDTO::fromRequest(request()->toArray()));
        }
        MatchUserToGroupJob::dispatch($user);

        return $user;
    }
}
