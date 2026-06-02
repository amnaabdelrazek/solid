<?php

namespace Modules\Lookups\Http\Resources;

use Illuminate\Http\Resources\Json\JsonResource;

class LookupValueResource extends JsonResource
{
    public function toArray($request): array
    {
        return [
            'id' => $this->id,
            'value_key' => $this->value_key,
            'label' => $this->getTranslatedValue('label'),
            'lookup_type' => new LookupTypeResource($this->whenLoaded('lookupType')),
        ];
    }
}
