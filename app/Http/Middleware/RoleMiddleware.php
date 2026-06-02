<?php

namespace App\Http\Middleware;

use Closure;
use Enums\UserRole;
use Illuminate\Http\Request;
use Symfony\Component\HttpFoundation\Response;

class RoleMiddleware
{
    public function handle(Request $request, Closure $next, string ...$roles): Response
    {
        $user = $request->user();

        abort_unless($user, 401);

        $allowed = collect($roles)->map(fn ($r) => UserRole::from($r));

        abort_unless($allowed->contains($user->role), 403, 'Insufficient permissions.');

        return $next($request);
    }
}
