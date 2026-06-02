<?php

namespace App\Support\Traits\Models;

use Illuminate\Database\Eloquent\Model;
use Illuminate\Support\Facades\App;

/**
 * @mixin Model
 */
trait HasTranslation
{
    /**
     * Define translatable columns in model
     */
    abstract protected function getTranslationColumns(): array;

    /**
     * Get current locale
     */
    protected function getLocale(): string
    {
        return App::getLocale(); // ar | en
    }

    /**
     * Handle dynamic attribute access
     */
    public function getAttribute($key)
    {
        // لو العمود من ضمن الترجمة
        if (in_array($key, $this->getTranslationColumns())) {
            return $this->getTranslatedValue($key);
        }

        return parent::getAttribute($key);
    }

    /**
     * Get translated value
     */
    public function getTranslatedValue(string $key)
    {
        $locale = $this->getLocale();

        $column = "{$key}_{$locale}";

        // fallback لو اللغة مش موجودة
        if (! array_key_exists($column, $this->attributes)) {
            $column = "{$key}_en";
        }

        return $this->attributes[$column] ?? null;
    }

    /**
     * Optional: return all translations
     */
    public function getAllTranslations(string $key): array
    {
        $translations = [];

        foreach ($this->attributes as $attribute => $value) {
            if (str_starts_with($attribute, $key.'_')) {
                $locale = str_replace($key.'_', '', $attribute);
                $translations[$locale] = $value;
            }
        }

        return $translations;
    }
}
