<?php

namespace App\Console\Commands;

use App\Settings\GeneralSettings;
use Carbon\Carbon;
use Illuminate\Console\Command;
use Illuminate\Support\Str;
use Modules\Groups\Enums\GroupStatus;
use Modules\Groups\Models\Group;
use Modules\Sessions\Models\Session;

class ScheduleWeeklySessions extends Command
{
    protected $signature = 'sessions:schedule-weekly
                            {--force : إعادة إنشاء الجلسات الموجودة مسبقاً لهذا الأسبوع}';

    protected $description = 'إنشاء الجلسات الأسبوعية لجميع المجموعات النشطة';

    public function handle(): int
    {
        $settings = app(GeneralSettings::class);

        $sessionDays = $settings->session_days;
        $startHour = $settings->session_start_hour;
        $durationMinutes = $settings->session_duration_minutes ?: 45;
        $price = $settings->session_price;

        if (empty($sessionDays)) {
            $this->warn('⚠️ لم يتم تحديد أيام الجلسات في الإعدادات.');

            return self::SUCCESS;
        }

        $groups = Group::where('status', GroupStatus::Active)
            ->whereNotNull('instructor_id')
            ->get();

        if ($groups->isEmpty()) {
            $this->info('✅ لا توجد مجموعات نشطة لإنشاء جلسات لها.');

            return self::SUCCESS;
        }

        $dayMapping = [
            'sunday' => Carbon::SUNDAY,
            'monday' => Carbon::MONDAY,
            'tuesday' => Carbon::TUESDAY,
            'wednesday' => Carbon::WEDNESDAY,
            'thursday' => Carbon::THURSDAY,
            'friday' => Carbon::FRIDAY,
            'saturday' => Carbon::SATURDAY,
        ];

        $days = array_values($sessionDays);
        $totalDays = count($days);
        $created = 0;
        $skipped = 0;
        $force = $this->option('force');

        foreach ($groups as $index => $group) {
            $dayKey = $days[$index % $totalDays];
            $weekDay = $dayMapping[$dayKey] ?? Carbon::SUNDAY;

            $scheduledAt = Carbon::now()->next($weekDay)->setTime($startHour, 0, 0);

            $existingSession = Session::where('group_id', $group->id)
                ->whereBetween('scheduled_at', [
                    $scheduledAt->copy()->startOfDay(),
                    $scheduledAt->copy()->endOfDay(),
                ])
                ->exists();

            if ($existingSession) {
                if (! $force) {
                    $this->line("⏭️ المجموعة {$group->id} لديها جلسة في هذا اليوم - تخطي.");
                    $skipped++;

                    continue;
                }

                Session::where('group_id', $group->id)
                    ->whereBetween('scheduled_at', [
                        $scheduledAt->copy()->startOfDay(),
                        $scheduledAt->copy()->endOfDay(),
                    ])
                    ->delete();
            }

            $maxSessionNumber = Session::where('group_id', $group->id)
                ->max('session_number');

            Session::create([
                'group_id' => $group->id,
                'instructor_id' => $group->instructor_id,
                'session_number' => ($maxSessionNumber ?? 0) + 1,
                'session_type' => 'paid',
                'status' => 'scheduled',
                'scheduled_at' => $scheduledAt,
                'duration_minutes' => $durationMinutes,
                'jitsi_room_name' => 'session-'.Str::uuid(),
                'session_metadata' => ['price' => $price],
            ]);

            $created++;
        }

        $this->info("✅ تم إنشاء {$created} جلسة (تم تخطي {$skipped}).");

        return self::SUCCESS;
    }
}
