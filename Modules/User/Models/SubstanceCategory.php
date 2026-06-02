<?php

namespace Modules\User\Models;

use App\Support\Traits\Models\HasTranslation;
use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Relations\HasMany;

class SubstanceCategory extends Model
{
    use HasTranslation;

    protected $fillable = [
        'name_ar',
        'name_en',
        'slug',
        'is_active',
        'sort_order',
    ];

    protected $casts = ['is_active' => 'boolean'];

    public function substances(): HasMany
    {
        return $this->hasMany(Substance::class);
    }

    protected function getTranslationColumns(): array
    {
        return ['name'];
    }
}
