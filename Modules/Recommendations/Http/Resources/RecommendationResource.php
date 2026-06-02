<?php

namespace Modules\Recommendations\Http\Resources;

use Illuminate\Http\Resources\Json\JsonResource;
use Modules\Lookups\Http\Resources\SubstanceCategoryResource;

class RecommendationResource extends JsonResource
{
    public function toArray($request): array
    {
        return [
            'id' => $this->id,
            'substance_category_id' => $this->substance_category_id,
            'type' => $this->type,
            'name_ar' => $this->name_ar,
            'name_en' => $this->name_en,
            'contact_info' => $this->contact_info,
            'latitude' => $this->latitude,
            'longitude' => $this->longitude,
            'is_active' => $this->is_active,
            'created_at' => $this->created_at,
            'updated_at' => $this->updated_at,
            'category' => new SubstanceCategoryResource($this->whenLoaded('category')),
        ];
    }
}
