<?php

use Illuminate\Support\Facades\Route;
use Modules\User\Http\Controllers\Api\UserController;

Route::middleware(['auth:sanctum'])->group(function () {
    Route::get('users/{user}', [UserController::class, 'show']);
    Route::put('profile', [UserController::class, 'update']);
    Route::get('instructors', [UserController::class, 'instructor']);
    Route::get('instructors/{user}', [UserController::class, 'showInstructor']);
});
