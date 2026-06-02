<?php

namespace Modules\Groups\Repositories;

interface UserSubstanceRepositoryInterface
{
    public function getCategoryIds(int $userId): array;
}
