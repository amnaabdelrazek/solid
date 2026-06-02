<?php

namespace Modules\Lookups\Models;

use App\Support\Traits\Models\HasTranslation;
use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Relations\BelongsTo;

class LookupValue extends Model
{
    use HasTranslation;

    protected $fillable = [
        'lookup_type_id',
        'value_key',
        'label_ar',
        'label_en',
        'sort_order',
        'is_active',
    ];

    protected $casts = ['is_active' => 'boolean'];

    public function type(): BelongsTo
    {
        return $this->belongsTo(LookupType::class, 'lookup_type_id');
    }

    protected function getTranslationColumns(): array
    {
        return ['label_ar', 'label_en'];
    }
}
