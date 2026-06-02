<?php

namespace Modules\Lookups\Http\Resources;

use Illuminate\Http\Resources\Json\JsonResource;

class SubstanceResource extends JsonResource
{
    public function toArray($request): array
    {
        return [
            'id' => $this->id,
            'label' => $this->getTranslatedValue('name'),
            'category' => new SubstanceCategoryResource($this->whenLoaded('category')),
        ];
    }
}
