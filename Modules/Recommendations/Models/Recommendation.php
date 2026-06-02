<?php

namespace Modules\Recommendations\Models;

use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Relations\BelongsTo;
use Modules\Recommendations\Enums\RecommendationType;
use Modules\User\Models\SubstanceCategory;

class Recommendation extends Model
{
    protected $fillable = [
        'substance_category_id',
        'type',
        'name_ar',
        'name_en',
        'contact_info',
        'latitude',
        'longitude',
        'is_active',
    ];

    protected $casts = [
        'type' => RecommendationType::class,
        'is_active' => 'boolean',
    ];

    public function category(): BelongsTo
    {
        return $this->belongsTo(SubstanceCategory::class, 'substance_category_id');
    }
}
