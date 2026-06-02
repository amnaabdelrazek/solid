<?php

namespace Modules\Sessions\Events;

use Illuminate\Broadcasting\InteractsWithSockets;
use Illuminate\Broadcasting\PrivateChannel;
use Illuminate\Contracts\Broadcasting\ShouldBroadcast;
use Illuminate\Foundation\Events\Dispatchable;
use Illuminate\Queue\SerializesModels;
use Modules\Sessions\Models\Session;

class SessionStartedEvent implements ShouldBroadcast
{
    use Dispatchable, InteractsWithSockets, SerializesModels;

    public function __construct(
        public readonly Session $session,
    ) {}

    public function broadcastOn(): array
    {
        return [
            new PrivateChannel("group.{$this->session->group_id}"),
            new PrivateChannel("session.{$this->session->id}"),
        ];
    }

    public function broadcastAs(): string
    {
        return 'session.started';
    }
}
