<?php

namespace App\Http\Resources;

use Illuminate\Contracts\Support\Arrayable;
use Illuminate\Http\Request;
use Illuminate\Http\Resources\Json\JsonResource;
use Modules\ActionEvent\Enums\ActionEventNameEnum;
use Modules\User\Models\User;

/**
 * @mixin User
 */
class CreatedByResource extends JsonResource
{
    /**
     * Transform the resource into an array.
     *
     * @param  Request  $request
     * @return array|Arrayable|\JsonSerializable
     */
    public function toArray($request)
    {

        return [
            'id' => $this->id,
            'name' => $this->name instanceof ActionEventNameEnum ? $this->name->label : $this->name,
            'email' => $this->email,
            'username' => $this->username,
            'phone' => $this->phone,
        ];
    }
}
