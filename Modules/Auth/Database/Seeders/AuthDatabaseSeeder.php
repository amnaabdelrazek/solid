<?php

namespace Modules\Auth\Database\Seeders;

use Illuminate\Database\Seeder;
use Modules\Auth\Enums\UserRole;
use Modules\User\Models\User;

class AuthDatabaseSeeder extends Seeder
{
    /**
     * Run the database seeds.
     */
    public function run(): void
    {
        User::query()->updateOrCreate(
            ['mobile_number' => '+201000000111'],
            [
                'display_name' => 'وليد السيسي',
                'username' => 'waleed.elsisi',
                'password' => bcrypt('Password@123'),
                'role' => UserRole::Instructor,
                'preferred_language' => 'ar',
                'is_active' => true,
            ]
        );
    }
}
