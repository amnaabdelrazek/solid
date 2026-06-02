<?php

namespace Modules\Groups\Listeners;

use Modules\Groups\Events\GroupReadyEvent;
use Modules\Notifications\Jobs\AdminAlertJob;

class NotifyAdminGroupReadyListener
{
    public function handle(GroupReadyEvent $event): void
    {
        AdminAlertJob::dispatch(
            "Group #{$event->group->id} is ready for session assignment",
            [
                'group_id' => $event->group->id,
                'group_type' => $event->group->group_type->value,
                'member_count' => $event->group->member_count,
            ],
        );
    }
}
