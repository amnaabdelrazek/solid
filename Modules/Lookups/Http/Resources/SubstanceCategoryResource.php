<?php

namespace Modules\Lookups\Http\Resources;

use Illuminate\Http\Resources\Json\JsonResource;

class SubstanceCategoryResource extends JsonResource
{
    public function toArray($request): array
    {
        return [
            'id' => $this->id,
            'label' => $this->getTranslatedValue('name'),
            'substances' => SubstanceResource::collection($this->whenLoaded('substances')),
        ];
    }
}
