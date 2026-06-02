<?php

namespace Modules\Lookups\Http\Resources;

use Illuminate\Http\Resources\Json\JsonResource;

class LookupTypeResource extends JsonResource
{
    public function toArray($request): array
    {
        return [
            'id' => $this->id,
            'key' => $this->key,
            'label' => $this->getTranslatedValue('label'),
            'values' => LookupValueResource::collection($this->whenLoaded('values')),
        ];
    }
}
