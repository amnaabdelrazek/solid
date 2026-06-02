<?php

use Illuminate\Support\Facades\Route;
use Modules\Lookups\Http\Controllers\LookupsController;

Route::middleware(['auth', 'verified'])->group(function () {
    Route::resource('lookups', LookupsController::class)->names('lookups');
});
