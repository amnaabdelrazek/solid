<?php

namespace Modules\Sessions\Services;

use App\Support\Traits\MakeAble;
use Firebase\JWT\JWT;
use Modules\Sessions\Models\Session;
use Modules\User\Models\User;

class JitsiTokenService
{
    use MakeAble;

    public function generate(Session $session, User $user): string
    {
        $payload = $this->buildPayload($session, $user);

        return JWT::encode($payload, $this->privateKey(), 'RS256');
    }

    private function buildPayload(Session $session, User $user): array
    {
        return [
            'iss' => config('jitsi.app_id'),
            'sub' => config('jitsi.domain'),
            'aud' => 'jitsi',
            'room' => $session->jitsi_room_name,
            'exp' => now()->addHour()->timestamp,
            'context' => $this->buildContext($user),
            'moderator' => $user->isInstructor(),
        ];
    }

    private function buildContext(User $user): array
    {
        return [
            'user' => [
                'id' => (string) $user->id,
                'name' => $user->display_name,
                'avatar' => '',
            ],
            'features' => [
                'livestreaming' => false,
                'recording' => false,
                'screen-sharing' => false,
                'outbound-call' => false,
            ],
        ];
    }

    private function privateKey(): string
    {
        return file_get_contents(config('jitsi.private_key_path'));
    }
}
