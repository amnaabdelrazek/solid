<?php

return [
    'app_id' => env('JITSI_APP_ID', 'addiction_recovery'),
    'domain' => env('JITSI_DOMAIN', 'meet.jit.si'),
    'server_url' => env('JITSI_SERVER_URL', 'https://meet.jit.si'),
    'private_key_path' => env('JITSI_PRIVATE_KEY_PATH', storage_path('jitsi/private.key')),
];
