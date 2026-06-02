<?php

namespace Modules\Lookups\Models;

use App\Support\Traits\Models\HasTranslation;
use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Relations\HasMany;

class LookupType extends Model
{
    use HasTranslation;

    protected $fillable = ['key', 'label_ar', 'label_en'];

    public function values(): HasMany
    {
        return $this->hasMany(LookupValue::class);
    }

    protected function getTranslationColumns(): array
    {
        return ['label_ar', 'label_en'];
    }
}
