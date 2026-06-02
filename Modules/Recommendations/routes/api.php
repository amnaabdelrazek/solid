<?php

use Illuminate\Support\Facades\Route;
use Modules\Recommendations\Http\Controllers\Api\RecommendationController;

Route::middleware('auth:sanctum')->group(function () {
    Route::get('recommendations', [RecommendationController::class, 'index']);
});
