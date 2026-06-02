<?php

namespace Modules\Auth\Actions;

use App\Support\Traits\MakeAble;
use Modules\Auth\DTOs\AddictionProfileDTO;
use Modules\User\Models\AddictionProfile;
use Modules\User\Models\User;

final class SaveAddictionProfileAction
{
    use MakeAble;

    public function execute(User $user, AddictionProfileDTO $dto): AddictionProfile
    {
        $profile = $this->createProfile($user, $dto);
        $this->syncSubstances($user, $dto->substanceIds);
        $this->syncTreatmentTypes($user, $dto->treatmentTypeIds);

        return $profile;
    }

    private function createProfile(User $user, AddictionProfileDTO $dto): AddictionProfile
    {
        return AddictionProfile::create([
            'user_id' => $user->id,
            'addiction_duration_id' => $dto->addictionDurationId,
            'education_level_id' => $dto->educationLevelId,
            'had_prior_treatment' => $dto->hadPriorTreatment,
            'addiction_reason' => $dto->addictionReason,
            'days_clean' => $dto->daysClean,
        ]);
    }

    private function syncSubstances(User $user, array $ids): void
    {
        $user->substances()->sync($ids);
    }

    private function syncTreatmentTypes(User $user, array $ids): void
    {
        $user->addictionProfile->treatmentTypes()->sync($ids);
    }
}
