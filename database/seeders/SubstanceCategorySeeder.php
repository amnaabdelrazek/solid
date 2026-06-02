<?php

namespace Database\Seeders;

use Illuminate\Database\Seeder;
use Modules\User\Models\SubstanceCategory;

class SubstanceCategorySeeder extends Seeder
{
    /**
     * Run the database seeds.
     */
    public function run(): void
    {
        /** @var array<int, array{slug: string, legacy_slug?: string, name_ar: string, name_en: string, sort_order: int}> $categories */
        $categories = [
            ['slug' => 'depressants', 'name_ar' => 'المثبطات', 'name_en' => 'Depressants', 'sort_order' => 1],
            ['slug' => 'sedatives', 'legacy_slug' => 'opioids', 'name_ar' => 'المهدئات', 'name_en' => 'Sedatives', 'sort_order' => 2],
            ['slug' => 'stimulants', 'name_ar' => 'المنشطات', 'name_en' => 'Stimulants', 'sort_order' => 3],
            ['slug' => 'hallucinogens', 'name_ar' => 'المهلوسات', 'name_en' => 'Hallucinogens', 'sort_order' => 4],
        ];

        foreach ($categories as $categoryData) {
            $category = SubstanceCategory::query()->where('slug', $categoryData['slug'])->first();

            if (! $category instanceof SubstanceCategory && isset($categoryData['legacy_slug'])) {
                $category = SubstanceCategory::query()->where('slug', $categoryData['legacy_slug'])->first();
            }

            $attributes = [
                'slug' => $categoryData['slug'],
                'name_ar' => $categoryData['name_ar'],
                'name_en' => $categoryData['name_en'],
                'sort_order' => $categoryData['sort_order'],
                'is_active' => true,
            ];

            if ($category instanceof SubstanceCategory) {
                $category->update($attributes);

                continue;
            }

            SubstanceCategory::query()->create($attributes);
        }
    }
}
