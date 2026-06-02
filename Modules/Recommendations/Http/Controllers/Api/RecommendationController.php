<?php

namespace Modules\Recommendations\Http\Controllers\Api;

use App\Http\Controllers\Api\ApiController;
use Illuminate\Http\JsonResponse;
use Illuminate\Http\Request;
use Modules\Recommendations\Http\Resources\RecommendationResource;
use Modules\Recommendations\Services\RecommendationService;

class RecommendationController extends ApiController
{
    public function __construct(
        private readonly RecommendationService $service,
    ) {
        parent::__construct();
    }

    public function index(Request $request): JsonResponse
    {
        $sessionId = $request->query('after_session');

        $recommendations = $sessionId
            ? $this->service->getAfterSession((int) $sessionId)
            : $this->service->getByCategory(null);

        return $this->apiBody([
            'recommendations' => RecommendationResource::collection($recommendations),
        ])->apiResponse();
    }
}
