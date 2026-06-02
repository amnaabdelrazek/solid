<?php

use Illuminate\Support\Facades\Route;
use Modules\Sessions\Http\Controllers\Api\SessionController;

Route::middleware('auth:sanctum')->group(function () {
    Route::get('sessions', [SessionController::class, 'index']);
    Route::get('sessions/me', [SessionController::class, 'meUpcomingSessions']);
    Route::get('sessions/{session}', [SessionController::class, 'show']);
    Route::post('sessions/{session}/join', [SessionController::class, 'join']);
    Route::post('sessions/{session}/leave', [SessionController::class, 'leave']);
    Route::post('sessions/{session}/feedback', [SessionController::class, 'feedback']);

    Route::group([], function () {
        //   Route::post('sessions', [SessionController::class, 'store']);
        Route::post('sessions/{session}/start', [SessionController::class, 'start']);
        Route::post('sessions/{session}/end', [SessionController::class, 'end']);
    });
});
