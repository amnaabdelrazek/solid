<?php

namespace Modules\Sessions\Models;

use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Relations\BelongsTo;
use Illuminate\Database\Eloquent\Relations\BelongsToMany;
use Modules\User\Models\User;

class BreakoutRoom extends Model
{
    protected $fillable = [
        'session_id',
        'room_name',
        'created_by',
        'is_open',
    ];

    protected $casts = ['is_open' => 'boolean'];

    public function session(): BelongsTo
    {
        return $this->belongsTo(Session::class);
    }

    public function creator(): BelongsTo
    {
        return $this->belongsTo(User::class, 'created_by');
    }

    public function members(): BelongsToMany
    {
        return $this->belongsToMany(
            User::class,
            'breakout_room_members'
        )->withPivot('assigned_at');
    }
}
