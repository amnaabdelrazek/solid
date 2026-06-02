<?php

use Illuminate\Support\Facades\Route;
use Modules\Sessions\Http\Controllers\SessionsController;

Route::middleware(['auth', 'verified'])->group(function () {
    Route::resource('sessions', SessionsController::class)->names('sessions');
});
