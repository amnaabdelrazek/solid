<?php

namespace Modules\Sessions\Models;

use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Relations\BelongsTo;
use Modules\User\Models\User;

class SessionAttendance extends Model
{
    protected $fillable = [
        'session_id',
        'user_id',
        'joined_at',
        'left_at',
        'was_present',
        'rating',
        'comment',
    ];

    protected $casts = [
        'joined_at' => 'datetime',
        'left_at' => 'datetime',
        'was_present' => 'boolean',
        'rating' => 'integer',
    ];

    public function session(): BelongsTo
    {
        return $this->belongsTo(Session::class);
    }

    public function user(): BelongsTo
    {
        return $this->belongsTo(User::class);
    }
}
