<?php

use App\Exceptions\ApiHandler;
use App\Http\Middleware\AppLanguage;
use Illuminate\Foundation\Application;
use Illuminate\Foundation\Configuration\Exceptions;
use Illuminate\Foundation\Configuration\Middleware;
use Illuminate\Http\Request;

return Application::configure(basePath: dirname(__DIR__))
    ->withRouting(
        web: __DIR__.'/../routes/web.php',
        api: __DIR__.'/../routes/api.php',
        commands: __DIR__.'/../routes/console.php',
        health: '/up',
    )
    ->withMiddleware(function (Middleware $middleware): void {
        $middleware->use([
            AppLanguage::class,
        ]);
    })
    ->withExceptions(function (Exceptions $exceptions): void {
        $apiHandler = new ApiHandler;

        $exceptions->render(function (Throwable $e, Request $request) use ($apiHandler) {
            return $apiHandler->handle($e, $request);
        });
    })->create();
