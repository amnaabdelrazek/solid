<?php

use Illuminate\Support\Facades\Route;
use Modules\Lookups\Http\Controllers\Api\LookupController;

Route::group([], function () {
    Route::get('lookups/substances', [LookupController::class, 'substances']);
    Route::get('lookups/{type}', [LookupController::class, 'byType']);
});
