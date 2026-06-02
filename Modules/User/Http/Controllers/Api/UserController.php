<?php

namespace Modules\User\Http\Controllers\Api;

use App\Http\Controllers\Api\ApiController;
use Illuminate\Http\JsonResponse;
use Illuminate\Http\Request;
use Modules\Auth\Http\Resources\UserResource;
use Modules\User\Http\Resources\InstructorResource;
use Modules\User\Models\User;

class UserController extends ApiController
{
    public function show(User $user): JsonResponse
    {
        return $this->apiBody([
            'user' => UserResource::make($user),
        ])->apiResponse();
    }

    public function update(Request $request): JsonResponse
    {
        $user = $request->user();

        $validated = $request->validate([
            'display_name' => 'sometimes|string|max:255',
            'email' => 'sometimes|email|unique:users,email,'.$user->id,
            'mobile_number' => ['sometimes', 'string', 'regex:/^\+?[1-9]\d{1,14}$/', 'unique:users,mobile_number,'.$user->id],
            'bio' => 'sometimes|nullable|string',
            'avatar_url' => 'sometimes|nullable|string',
        ]);

        $user->update(array_filter($validated, fn ($value) => ! empty($value)));

        return $this->apiBody([
            'user' => UserResource::make($user),
        ])->apiMessage('Profile updated successfully.')->apiResponse();
    }

    public function instructor(): JsonResponse
    {
        return $this->apiBody([
            'instructors' => InstructorResource::collection(User::query()->where('role', 'instructor')->get()),
        ])->apiResponse();
    }

    public function showInstructor(User $user): JsonResponse
    {
        if (! $user->isInstructor()) {
            return $this->apiMessage('User is not an instructor.')->apiCode(404)->apiResponse();
        }

        return $this->apiBody([
            'instructor' => InstructorResource::make($user),
        ])->apiResponse();
    }
}
