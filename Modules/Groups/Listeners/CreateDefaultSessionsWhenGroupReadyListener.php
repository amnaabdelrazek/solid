<?php

namespace Modules\Groups\Listeners;

use App\Settings\GeneralSettings;
use Carbon\Carbon;
use Modules\Groups\Events\GroupReadyEvent;
use Modules\Sessions\Models\Session;

class CreateDefaultSessionsWhenGroupReadyListener
{
    public function handle(GroupReadyEvent $event): void
    {
        $group = $event->group;

        if (Session::query()->where('group_id', $group->id)->exists()) {
            return;
        }

        if ($group->instructor_id === null) {
            return;
        }

        $settings = app(GeneralSettings::class);
        $baseDate = Carbon::now();
        $startHour = $settings->session_start_hour;
        $durationMinutes = $settings->session_duration_minutes;
        $price = $settings->session_price;
        $sessionDays = $settings->session_days;

        $dayMapping = [
            'sunday' => Carbon::SUNDAY,
            'monday' => Carbon::MONDAY,
            'tuesday' => Carbon::TUESDAY,
            'wednesday' => Carbon::WEDNESDAY,
            'thursday' => Carbon::THURSDAY,
            'friday' => Carbon::FRIDAY,
            'saturday' => Carbon::SATURDAY,
        ];

        foreach ($sessionDays as $index => $dayKey) {
            $weekDay = $dayMapping[$dayKey] ?? Carbon::SUNDAY;
            $scheduledAt = $baseDate->copy()->next($weekDay)->setTime($startHour, 0, 0);

            Session::query()->create([
                'group_id' => $group->id,
                'instructor_id' => $group->instructor_id,
                'session_number' => $index + 1,
                'session_type' => 'paid',
                'status' => 'scheduled',
                'scheduled_at' => $scheduledAt,
                'duration_minutes' => $durationMinutes,
                'jitsi_room_name' => sprintf('group-%d-session-%d', $group->id, $index + 1),
                'session_metadata' => ['price' => $price],
            ]);
        }
    }
}
