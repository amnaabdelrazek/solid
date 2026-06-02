<?php

namespace Modules\Sessions\Listeners;

use Modules\Notifications\Jobs\SendPushNotificationJob;
use Modules\Sessions\Events\SessionEndedEvent;

class SendPostSessionRecommendationsListener
{
    public function handle(SessionEndedEvent $event): void
    {
        $session = $event->session->load('group.members');

        $session->group->members->each(function ($user) {
            SendPushNotificationJob::dispatch(
                $user,
                'Session Complete',
                'View personalized recommendations for your recovery journey.',
            );
        });
    }
}
