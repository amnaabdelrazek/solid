<?php

namespace Database\Seeders;

use Illuminate\Database\Seeder;
use Modules\User\Models\Substance;
use Modules\User\Models\SubstanceCategory;

class SubstanceSeeder extends Seeder
{
    /**
     * Run the database seeds.
     */
    public function run(): void
    {
        /** @var array<string, array<int, array{name_ar: string, name_en: string, aliases?: array<int, string>}>> $substances */
        $substances = [
            'depressants' => [
                ['name_ar' => 'حشيش', 'name_en' => 'Hash', 'aliases' => ['Hashish']],
                ['name_ar' => 'بانجو', 'name_en' => 'Bango'],
                ['name_ar' => 'هيدرا', 'name_en' => 'Hydra', 'aliases' => ['Hydro']],
            ],
            'sedatives' => [
                ['name_ar' => 'أفيون', 'name_en' => 'Opium'],
                ['name_ar' => 'ترامادول', 'name_en' => 'Tramadol'],
                ['name_ar' => 'هيروين', 'name_en' => 'Heroin'],
            ],
            'stimulants' => [
                ['name_ar' => 'شابو', 'name_en' => 'Shabu'],
                ['name_ar' => 'كوكايين', 'name_en' => 'Cocaine'],
                ['name_ar' => 'إكستاسي', 'name_en' => 'Ecstasy'],
            ],
            'hallucinogens' => [
                ['name_ar' => 'إل إس دي', 'name_en' => 'LSD'],
                ['name_ar' => 'آيس (كريستال ميث)', 'name_en' => 'Ice (Crystal Meth)', 'aliases' => ['Ice']],
            ],
        ];

        $activeSubstanceIds = [];
        $activeCategoryIds = [];

        foreach ($substances as $categorySlug => $categorySubstances) {
            $category = SubstanceCategory::query()->where('slug', $categorySlug)->firstOrFail();
            $activeCategoryIds[] = $category->id;

            foreach ($categorySubstances as $substanceData) {
                $substance = Substance::query()
                    ->whereIn('name_en', array_merge([$substanceData['name_en']], $substanceData['aliases'] ?? []))
                    ->first();

                $attributes = [
                    'substance_category_id' => $category->id,
                    'name_ar' => $substanceData['name_ar'],
                    'name_en' => $substanceData['name_en'],
                    'is_active' => true,
                ];

                if ($substance instanceof Substance) {
                    $substance->update($attributes);
                } else {
                    $substance = Substance::query()->create($attributes);
                }

                $activeSubstanceIds[] = $substance->id;
            }
        }

        Substance::query()
            ->whereIn('substance_category_id', $activeCategoryIds)
            ->whereNotIn('id', $activeSubstanceIds)
            ->update(['is_active' => false]);
    }
}
