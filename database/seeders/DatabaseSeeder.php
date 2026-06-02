<?php

namespace Database\Seeders;

use Illuminate\Database\Seeder;
use Illuminate\Support\Facades\Hash;
use Modules\Auth\Database\Seeders\AuthDatabaseSeeder;
use Modules\Auth\Enums\UserRole;
use Modules\Lookups\Models\LookupType;
use Modules\Lookups\Models\LookupValue;
use Modules\User\Models\User;

class DatabaseSeeder extends Seeder
{
    public function run(): void
    {
        $this->seedLookupTypes();
        $this->call([
            SubstanceCategorySeeder::class,
            SubstanceSeeder::class,
            AuthDatabaseSeeder::class,
        ]);

        $this->seedAdminUser();

        $this->call([
            SessionPaymentSeeder::class,
        ]);
    }

    private function seedAdminUser(): void
    {
        User::firstOrCreate(
            ['mobile_number' => '966500000000'],
            [
                'display_name' => 'Admin',
                'username' => 'admin',
                'password' => Hash::make('password'),
                'role' => UserRole::Admin,
                'preferred_language' => 'ar',
                'is_active' => true,
            ],
        );
    }

    private function seedLookupTypes(): void
    {
        $types = [
            ['key' => 'addiction_duration', 'label_ar' => 'مدة الإدمان', 'label_en' => 'Addiction Duration'],
            ['key' => 'education_level', 'label_ar' => 'المستوى التعليمي', 'label_en' => 'Education Level'],
            ['key' => 'treatment_type', 'label_ar' => 'نوع العلاج', 'label_en' => 'Treatment Type'],
        ];

        foreach ($types as $typeData) {
            $type = LookupType::firstOrCreate(['key' => $typeData['key']], $typeData);
            $this->seedLookupValues($type);
        }
    }

    private function seedLookupValues(LookupType $type): void
    {
        $values = match ($type->key) {
            'addiction_duration' => [
                ['value_key' => 'less_6m', 'label_ar' => 'أقل من 6 أشهر', 'label_en' => 'Less than 6 months', 'sort_order' => 1],
                ['value_key' => '6_12m', 'label_ar' => '6 - 12 شهر', 'label_en' => '6 - 12 months', 'sort_order' => 2],
                ['value_key' => '1_3y', 'label_ar' => '1 - 3 سنوات', 'label_en' => '1 - 3 years', 'sort_order' => 3],
                ['value_key' => 'over_3y', 'label_ar' => 'أكثر من 3 سنوات', 'label_en' => 'Over 3 years', 'sort_order' => 4],
            ],
            'education_level' => [
                ['value_key' => 'none', 'label_ar' => 'بدون تعليم', 'label_en' => 'No Education', 'sort_order' => 1],
                ['value_key' => 'primary', 'label_ar' => 'ابتدائي', 'label_en' => 'Primary', 'sort_order' => 2],
                ['value_key' => 'secondary', 'label_ar' => 'ثانوي', 'label_en' => 'Secondary', 'sort_order' => 3],
                ['value_key' => 'university', 'label_ar' => 'جامعي', 'label_en' => 'University', 'sort_order' => 4],
                ['value_key' => 'postgraduate', 'label_ar' => 'دراسات عليا', 'label_en' => 'Postgraduate', 'sort_order' => 5],
            ],
            'treatment_type' => [
                ['value_key' => 'hospital', 'label_ar' => 'علاج في المستشفى', 'label_en' => 'Hospital Treatment', 'sort_order' => 1],
                ['value_key' => 'outpatient', 'label_ar' => 'علاج خارجي', 'label_en' => 'Outpatient', 'sort_order' => 2],
                ['value_key' => 'self', 'label_ar' => 'علاج ذاتي', 'label_en' => 'Self Treatment', 'sort_order' => 3],
                ['value_key' => 'religious', 'label_ar' => 'علاج ديني', 'label_en' => 'Religious', 'sort_order' => 4],
            ],
            default => [],
        };

        foreach ($values as $value) {
            LookupValue::firstOrCreate(
                ['lookup_type_id' => $type->id, 'value_key' => $value['value_key']],
                array_merge($value, ['lookup_type_id' => $type->id, 'is_active' => true]),
            );
        }
    }
}
