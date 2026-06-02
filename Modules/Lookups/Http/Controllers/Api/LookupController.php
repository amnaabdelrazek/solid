<?php

namespace Modules\Lookups\Http\Controllers\Api;

use App\Http\Controllers\Api\ApiController;
use Illuminate\Http\JsonResponse;
use Modules\Lookups\Http\Resources\LookupValueResource;
use Modules\Lookups\Http\Resources\SubstanceCategoryResource;
use Modules\Lookups\Models\LookupType;
use Modules\User\Models\SubstanceCategory;

class LookupController extends ApiController
{
    public function substances(): JsonResponse
    {
        $categories = SubstanceCategory::where('is_active', true)
            ->with(['substances' => fn ($q) => $q->where('is_active', true)])
            ->orderBy('sort_order')
            ->get();

        return $this->apiBody([
            'categories' => SubstanceCategoryResource::collection($categories),
        ])->apiResponse();
    }

    public function byType(string $type): JsonResponse
    {
        $lookupType = LookupType::where('key', $type)->firstOrFail();

        $values = $lookupType->values()
            ->where('is_active', true)
            ->orderBy('sort_order')
            ->get();

        return $this->apiBody([
            'values' => LookupValueResource::collection($values),
        ])->apiResponse();
    }
}
