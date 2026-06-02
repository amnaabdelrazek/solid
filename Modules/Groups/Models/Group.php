<?php

namespace Modules\Groups\Models;

use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Relations\BelongsTo;
use Illuminate\Database\Eloquent\Relations\BelongsToMany;
use Illuminate\Database\Eloquent\Relations\HasMany;
use Illuminate\Database\Eloquent\SoftDeletes;
use Modules\Groups\Enums\GroupStatus;
use Modules\Groups\Enums\GroupType;
use Modules\Sessions\Models\Session;
use Modules\User\Models\User;

class Group extends Model
{
    use SoftDeletes;

    protected $fillable = [
        'instructor_id',
        'substance_category_id',
        'group_type',
        'status',
        'name_ar',
        'name_en',
        'min_members',
        'max_members',
    ];

    protected $casts = [
        'group_type' => GroupType::class,
        'status' => GroupStatus::class,
    ];

    public function instructor(): BelongsTo
    {
        return $this->belongsTo(User::class, 'instructor_id');
    }

    public function members(): BelongsToMany
    {
        return $this->belongsToMany(User::class, 'group_members')
            ->withPivot(['joined_at', 'is_active'])
            ->wherePivot('is_active', 1);
    }

    public function sessions(): HasMany
    {
        return $this->hasMany(Session::class);
    }

    public function getMemberCountAttribute(): int
    {
        return $this->members()->count();
    }
}
