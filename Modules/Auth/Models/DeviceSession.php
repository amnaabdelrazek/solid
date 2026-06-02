<?php

namespace Modules\Auth\Models;

use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Relations\BelongsTo;
use Modules\Auth\Enums\DeviceEventType;
use Modules\User\Models\User;

class DeviceSession extends Model
{
    public $timestamps = false;

    protected $fillable = [
        'user_id',
        'device_id',
        'device_info',
        'event_type',
        'sanctum_token_id',
        'created_at',
    ];

    protected $casts = [
        'event_type' => DeviceEventType::class,
        'device_info' => 'array',
        'created_at' => 'datetime',
    ];

    public function user(): BelongsTo
    {
        return $this->belongsTo(User::class);
    }
}
