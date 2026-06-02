<?php

namespace Modules\Recommendations\Repositories;

use Illuminate\Database\Eloquent\Collection;

interface RecommendationRepositoryInterface
{
    public function forSession(int $sessionId): Collection;

    public function forCategory(?int $categoryId): Collection;
}
