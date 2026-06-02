<?php

namespace App\Http\Controllers\Api;

use App\Settings\ContentSettings;
use App\Settings\GeneralSettings;
use Illuminate\Http\JsonResponse;
use Illuminate\Http\Request;
use ReflectionClass;

class SettingsController extends ApiController
{
    public function index(): JsonResponse
    {
        $settings = app(GeneralSettings::class);
        $reflection = new ReflectionClass($settings);
        $properties = [];

        foreach ($reflection->getProperties() as $property) {
            $name = $property->getName();
            $properties[$name] = [
                'value' => $settings->$name,
                'type' => $property->getType()?->getName(),
            ];
        }

        return $this->apiBody([
            'settings' => $properties,
        ])->apiResponse();
    }

    public function show(string $key): JsonResponse
    {
        $settings = app(GeneralSettings::class);
        $reflection = new ReflectionClass($settings);

        abort_unless($reflection->hasProperty($key), 404, "Setting '{$key}' not found.");

        $property = $reflection->getProperty($key);

        return $this->apiBody([
            'setting' => [
                'key' => $key,
                'value' => $settings->$key,
                'type' => $property->getType()?->getName(),
            ],
        ])->apiResponse();
    }

    public function update(Request $request, string $key): JsonResponse
    {
        $settings = app(GeneralSettings::class);
        $reflection = new ReflectionClass($settings);

        abort_unless($reflection->hasProperty($key), 404, "Setting '{$key}' not found.");

        $property = $reflection->getProperty($key);
        $type = $property->getType()?->getName();

        $validated = $request->validate([
            'value' => ['required'],
        ]);

        $value = match ($type) {
            'int' => (int) $validated['value'],
            'array' => is_array($validated['value']) ? $validated['value'] : json_decode($validated['value'], true),
            default => $validated['value'],
        };

        $settings->$key = $value;
        $settings->save();

        return $this->apiMessage('Setting updated successfully.')->apiResponse();
    }

    public function privacyPolicy(): JsonResponse
    {
        $settings = app(ContentSettings::class);

        return $this->apiBody([
            'privacy_policy' => $settings->privacy_policy,
        ])->apiResponse();
    }

    public function termsAndConditions(): JsonResponse
    {
        $settings = app(ContentSettings::class);

        return $this->apiBody([
            'terms_and_conditions' => $settings->terms_and_conditions,
        ])->apiResponse();
    }
}
