<?php

namespace Modules\Recommendations\Services;

use App\Support\Traits\MakeAble;
use Illuminate\Database\Eloquent\Collection;
use Modules\Recommendations\Repositories\RecommendationRepositoryInterface;

final class RecommendationService
{
    use MakeAble;

    public function __construct(
        private readonly RecommendationRepositoryInterface $recommendations,
    ) {}

    public function getAfterSession(int $sessionId): Collection
    {
        return $this->recommendations->forSession($sessionId);
    }

    public function getByCategory(?int $categoryId): Collection
    {
        return $this->recommendations->forCategory($categoryId);
    }
}
