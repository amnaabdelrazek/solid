<?php

namespace Modules\User\Models;

use App\Support\Traits\Models\HasTranslation;
use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Relations\BelongsTo;

class Substance extends Model
{
    use HasTranslation;

    protected $fillable = [
        'substance_category_id',
        'name_ar',
        'name_en',
        'is_active',
    ];

    protected $casts = ['is_active' => 'boolean'];

    public function category(): BelongsTo
    {
        return $this->belongsTo(SubstanceCategory::class, 'substance_category_id');
    }

    protected function getTranslationColumns(): array
    {
        return ['name'];
    }
}
