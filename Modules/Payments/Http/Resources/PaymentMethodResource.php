<?php

namespace Modules\Payments\Http\Resources;

use Illuminate\Http\Resources\Json\JsonResource;

class PaymentMethodResource extends JsonResource
{
    public function toArray($request): array
    {
        return [
            'id' => $this->id,
            'card_holder' => $this->card_type,
            'card_number' => $this->card_number,
            'expiry' => $this->expiry,
            'is_default' => $this->is_default,
        ];
    }
}
