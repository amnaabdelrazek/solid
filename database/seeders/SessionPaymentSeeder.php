<?php

namespace Database\Seeders;

use Illuminate\Database\Seeder;
use Illuminate\Support\Collection;
use Illuminate\Support\Facades\DB;
use Illuminate\Support\Facades\Hash;
use Modules\Auth\Enums\UserRole;
use Modules\Groups\Enums\GroupStatus;
use Modules\Groups\Enums\GroupType;
use Modules\Groups\Models\Group;
use Modules\Payments\Enums\PaymentStatus;
use Modules\Payments\Models\Payment;
use Modules\Sessions\Enums\SessionStatus;
use Modules\Sessions\Enums\SessionType;
use Modules\Sessions\Models\Session;
use Modules\User\Models\SubstanceCategory;
use Modules\User\Models\User;

class SessionPaymentSeeder extends Seeder
{
    public function run(): void
    {
        $instructor = $this->ensureInstructor();
        $users = $this->createAddictUsers();
        $categories = $this->getCategories();
        $groups = $this->createGroups($categories, $instructor);
        $this->assignUsersToGroups($users, $groups);
        $sessions = $this->createSessions($groups, $instructor);
        $this->createPayments($users, $sessions);
    }

    private function ensureInstructor(): User
    {
        $instructor = User::where('role', UserRole::Instructor)->first();

        if (! $instructor) {
            $instructor = User::create([
                'display_name' => 'Instructor',
                'username' => 'instructor',
                'mobile_number' => '966500000010',
                'password' => Hash::make('password'),
                'role' => UserRole::Instructor,
                'preferred_language' => 'ar',
                'is_active' => true,
            ]);
        }

        return $instructor;
    }

    /**
     * @return Collection<int, User>
     */
    private function createAddictUsers(): Collection
    {
        $existing = User::where('role', UserRole::Addict)->get();

        if ($existing->isNotEmpty()) {
            return $existing;
        }

        $users = collect();
        $names = [
            ['display_name' => 'أحمد علي', 'username' => 'ahmed.ali'],
            ['display_name' => 'محمد حسن', 'username' => 'mohamed.hassan'],
            ['display_name' => 'سارة خالد', 'username' => 'sara.khaled'],
            ['display_name' => 'فاطمة عمر', 'username' => 'fatima.omar'],
            ['display_name' => 'خالد يوسف', 'username' => 'khaled.yousef'],
        ];

        foreach ($names as $i => $data) {
            $user = User::firstOrCreate(
                [
                    'mobile_number' => '9665000000'.($i + 11),
                    'username' => $data['username'],
                ],
                [

                    'display_name' => $data['display_name'],
                    'password' => Hash::make('password'),
                    'role' => UserRole::Addict,
                    'preferred_language' => 'ar',
                    'is_active' => true,
                ],
            );
            $users->push($user);
        }

        return $users;
    }

    /**
     * @return Collection<int, SubstanceCategory>
     */
    private function getCategories(): Collection
    {
        return SubstanceCategory::all();
    }

    /**
     * @param  Collection<int, SubstanceCategory>  $categories
     * @return Collection<int, Group>
     */
    private function createGroups(Collection $categories, User $instructor): Collection
    {
        $groups = collect();

        foreach ($categories as $category) {
            $group = Group::firstOrCreate(
                [
                    'substance_category_id' => $category->id,
                    'instructor_id' => $instructor->id,
                    'group_type' => GroupType::SingleCategory,
                ],
                [
                    'status' => GroupStatus::Active,
                    'name_ar' => 'مجموعة '.$category->name_ar,
                    'name_en' => $category->name_en.' Group',
                    'min_members' => 5,
                    'max_members' => 15,
                ],
            );
            $groups->push($group);
        }

        $mixedGroup = Group::firstOrCreate(
            [
                'group_type' => GroupType::Mixed,
                'instructor_id' => $instructor->id,
                'substance_category_id' => null,
            ],
            [
                'status' => GroupStatus::Active,
                'name_ar' => 'مجموعة متنوعة',
                'name_en' => 'Mixed Group',
                'min_members' => 5,
                'max_members' => 15,
            ],
        );
        $groups->push($mixedGroup);

        return $groups;
    }

    /**
     * @param  Collection<int, User>  $users
     * @param  Collection<int, Group>  $groups
     */
    private function assignUsersToGroups(Collection $users, Collection $groups): void
    {
        foreach ($groups as $group) {
            foreach ($users as $user) {
                DB::table('group_members')->updateOrInsert(
                    ['group_id' => $group->id, 'user_id' => $user->id],
                    ['joined_at' => now()->subDays(rand(30, 90)), 'is_active' => true],
                );
            }
        }
    }

    /**
     * @param  Collection<int, Group>  $groups
     * @return Collection<int, Session>
     */
    private function createSessions(Collection $groups, User $instructor): Collection
    {
        $sessions = collect();

        foreach ($groups as $group) {
            for ($i = 1; $i <= 3; $i++) {
                $scheduledAt = now()->addDays(($i - 1) * 10 + rand(1, 5));
                $session = Session::firstOrCreate(
                    ['jitsi_room_name' => 'room_'.$group->id.'_'.$i.'_'.md5($group->id.'_'.$i)],
                    [
                        'group_id' => $group->id,
                        'instructor_id' => $instructor->id,
                        'session_number' => $i,
                        'session_type' => SessionType::Paid,
                        'status' => SessionStatus::Finished,
                        'scheduled_at' => $scheduledAt,
                        'started_at' => $scheduledAt,
                        'ended_at' => $scheduledAt->copy()->addMinutes(45),
                        'duration_minutes' => 45,
                    ],
                );
                $sessions->push($session);
            }
        }

        return $sessions;
    }

    /**
     * @param  Collection<int, User>  $users
     * @param  Collection<int, Session>  $sessions
     */
    private function createPayments(Collection $users, Collection $sessions): void
    {
        foreach ($users as $user) {
            $availableSessions = $sessions->shuffle();

            for ($i = 0; $i < 10; $i++) {
                $session = $availableSessions->get($i % $availableSessions->count());
                $daysAgo = rand(1, 60);

                Payment::firstOrCreate(
                    [
                        'user_id' => $user->id,
                        'session_id' => $session->id,
                        'gateway_transaction_id' => 'TXN_'.$user->id.'_'.$session->id.'_'.$i,
                    ],
                    [
                        'amount' => rand(500, 2000),
                        'currency' => 'EGP',
                        'status' => PaymentStatus::Paid,
                        'gateway' => 'cash',
                        'paid_at' => now()->subDays($daysAgo),
                    ],
                );
            }
        }
    }
}
