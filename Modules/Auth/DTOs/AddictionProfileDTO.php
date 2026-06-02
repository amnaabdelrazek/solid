<?php

namespace Modules\Auth\DTOs;

final readonly class AddictionProfileDTO
{
    public function __construct(
        public int $addictionDurationId,
        public int $educationLevelId,
        public bool $hadPriorTreatment,
        public array $substanceIds,
        public array $treatmentTypeIds = [],
        public ?string $addictionReason = null,
        public ?int $daysClean = null,
    ) {}

    public static function fromRequest(array $data): self
    {
        return new self(
            addictionDurationId: $data['addiction_duration_id'],
            educationLevelId: $data['education_level_id'],
            hadPriorTreatment: (bool) $data['had_prior_treatment'],
            substanceIds: $data['substance_ids'],
            treatmentTypeIds: $data['treatment_type_ids'] ?? [],
            addictionReason: $data['addiction_reason'] ?? null,
            daysClean: $data['days_clean'] ?? null,
        );
    }
}
