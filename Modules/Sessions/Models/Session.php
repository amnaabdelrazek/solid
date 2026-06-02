<?php

namespace Modules\Sessions\Models;

use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Relations\BelongsTo;
use Illuminate\Database\Eloquent\Relations\HasMany;
use Illuminate\Database\Eloquent\SoftDeletes;
use Modules\Groups\Models\Group;
use Modules\Payments\Models\Payment;
use Modules\Sessions\Enums\SessionStatus;
use Modules\Sessions\Enums\SessionType;
use Modules\User\Models\User;

class Session extends Model
{
    use SoftDeletes;

    protected $table = 'therapy_sessions';

    protected $fillable = [
        'group_id',
        'instructor_id',
        'session_number',
        'session_type',
        'status',
        'scheduled_at',
        'started_at',
        'ended_at',
        'duration_minutes',
        'jitsi_room_name',
        'jitsi_jwt_issued_at',
        'session_metadata',
    ];

    protected $casts = [
        'session_type' => SessionType::class,
        'status' => SessionStatus::class,
        'scheduled_at' => 'datetime',
        'started_at' => 'datetime',
        'ended_at' => 'datetime',
        'jitsi_jwt_issued_at' => 'datetime',
        'session_metadata' => 'array',
    ];

    public function group(): BelongsTo
    {
        return $this->belongsTo(Group::class);
    }

    public function instructor(): BelongsTo
    {
        return $this->belongsTo(User::class, 'instructor_id');
    }

    public function attendances(): HasMany
    {
        return $this->hasMany(SessionAttendance::class);
    }

    public function payments(): HasMany
    {
        return $this->hasMany(Payment::class);
    }
}
