<?php

use Illuminate\Support\Facades\Route;
use Modules\Recommendations\Http\Controllers\RecommendationsController;

Route::middleware(['auth', 'verified'])->group(function () {
    Route::resource('recommendations', RecommendationsController::class)->names('recommendations');
});
