<?php

use App\Http\Controllers\Api\SettingsController;
use Illuminate\Http\Request;
use Illuminate\Support\Facades\Route;

Route::get('/user', function (Request $request) {
    return $request->user();
})->middleware('auth:sanctum');

Route::middleware(['auth:sanctum'])->prefix('v1')->group(function () {
    Route::get('/settings', [SettingsController::class, 'index']);
    Route::get('/settings/{key}', [SettingsController::class, 'show']);
    Route::put('/settings/{key}', [SettingsController::class, 'update']);
});

Route::get('/privacy-policy', [SettingsController::class, 'privacyPolicy']);
Route::get('/terms-and-conditions', [SettingsController::class, 'termsAndConditions']);
