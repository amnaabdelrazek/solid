<?php

use Illuminate\Support\Facades\Route;
use Modules\Groups\Http\Controllers\Api\GroupController;

Route::middleware('auth:sanctum')->group(function () {
    Route::get('groups/my', [GroupController::class, 'myGroup']);
    Route::get('groups/', [GroupController::class, 'index']);
    Route::post('groups/subscribe', [GroupController::class, 'subscribe']);
});
