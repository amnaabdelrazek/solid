<?php

namespace Modules\Payments\Models;

use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Relations\BelongsTo;
use Modules\User\Models\User;

class PaymentMethod extends Model
{
    protected $fillable = [
        'user_id',
        'card_type',
        'card_number',
        'expiry',
        'is_default',
        'gateway_token',
    ];

    protected $casts = [
        'is_default' => 'boolean',
    ];

    public function user(): BelongsTo
    {
        return $this->belongsTo(User::class);
    }
}
