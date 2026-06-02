<?php

namespace Modules\Recommendations\Repositories;

use Illuminate\Database\Eloquent\Collection;
use Modules\Recommendations\Models\Recommendation;
use Modules\Sessions\Models\Session;

final class EloquentRecommendationRepository implements RecommendationRepositoryInterface
{
    public function forSession(int $sessionId): Collection
    {
        $session = Session::with('group')->findOrFail($sessionId);
        $categoryId = $session->group->substance_category_id;

        return $this->forCategory($categoryId);
    }

    public function forCategory(?int $categoryId): Collection
    {
        return Recommendation::where('is_active', true)
            ->where(function ($q) use ($categoryId) {
                $q->whereNull('substance_category_id')
                    ->orWhere('substance_category_id', $categoryId);
            })
            ->orderBy('type')
            ->get();
    }
}
