<?php

namespace Modules\Payments\Models;

use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Relations\BelongsTo;
use Modules\Payments\Enums\PaymentStatus;
use Modules\Sessions\Models\Session;
use Modules\User\Models\User;

class Payment extends Model
{
    protected $fillable = [
        'user_id',
        'session_id',
        'amount',
        'currency',
        'status',
        'gateway',
        'gateway_transaction_id',
        'gateway_response',
        'paid_at',
    ];

    protected $casts = [
        'status' => PaymentStatus::class,
        'gateway_response' => 'array',
        'paid_at' => 'datetime',
        'amount' => 'decimal:2',
    ];

    public function user(): BelongsTo
    {
        return $this->belongsTo(User::class);
    }

    public function session(): BelongsTo
    {
        return $this->belongsTo(Session::class);
    }
}
