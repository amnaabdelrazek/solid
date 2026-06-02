<?php

use Illuminate\Support\Facades\Route;
use Modules\Groups\Http\Controllers\GroupsController;

Route::middleware(['auth', 'verified'])->group(function () {
    Route::resource('groups', GroupsController::class)->names('groups');
});
