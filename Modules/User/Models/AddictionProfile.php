<?php

namespace Modules\User\Models;

use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Relations\BelongsTo;
use Illuminate\Database\Eloquent\Relations\BelongsToMany;
use Modules\Lookups\Models\LookupValue;

class AddictionProfile extends Model
{
    protected $fillable = [
        'user_id',
        'addiction_duration_id',
        'education_level_id',
        'had_prior_treatment',
        'addiction_reason',
        'days_clean',
    ];

    protected $casts = [
        'had_prior_treatment' => 'boolean',
    ];

    public function user(): BelongsTo
    {
        return $this->belongsTo(User::class);
    }

    public function addictionDuration(): BelongsTo
    {
        return $this->belongsTo(LookupValue::class, 'addiction_duration_id');
    }

    public function educationLevel(): BelongsTo
    {
        return $this->belongsTo(LookupValue::class, 'education_level_id');
    }

    public function treatmentTypes(): BelongsToMany
    {
        return $this->belongsToMany(
            LookupValue::class,
            'user_treatment_types',
            'user_id',
            'lookup_value_id'
        );
    }
}
