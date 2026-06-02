<?php

namespace Modules\Auth\Http\Controllers\Api;

use App\Http\Controllers\Api\ApiController;
use Illuminate\Http\JsonResponse;
use Illuminate\Http\Request;
use Modules\Auth\Actions\DeleteAccountAction;
use Modules\Auth\Actions\ForgotPasswordAction;
use Modules\Auth\Actions\GenerateTokenForRegrettedUserAction;
use Modules\Auth\Actions\LoginAction;
use Modules\Auth\Actions\LogoutAction;
use Modules\Auth\Actions\RegisterUserAction;
use Modules\Auth\Actions\ResetPasswordAction;
use Modules\Auth\DTOs\LoginDTO;
use Modules\Auth\DTOs\RegisterUserDTO;
use Modules\Auth\Events\PasswordResetRequested;
use Modules\Auth\Events\UserRegistered;
use Modules\Auth\Http\Requests\ForgotPasswordRequest;
use Modules\Auth\Http\Requests\LoginRequest;
use Modules\Auth\Http\Requests\RegisterRequest;
use Modules\Auth\Http\Requests\ResetPasswordRequest;
use Modules\Auth\Http\Requests\VerifyForgotPasswordOtpRequest;
use Modules\Auth\Http\Requests\VerifyRegisterOtpRequest;
use Modules\Auth\Http\Resources\UserResource;
use Modules\Auth\Services\VerifyForgotPasswordOtpService;
use Modules\Auth\Services\VerifyRegisterOtpService;
use Modules\User\Models\User;

class AuthController extends ApiController
{
    public function register(
        RegisterRequest $request,
        RegisterUserAction $action,
        GenerateTokenForRegrettedUserAction $regrettedUserAction
    ): JsonResponse {
        $user = $action->execute(RegisterUserDTO::fromRequest($request->validated()));
        $token = $regrettedUserAction->execute($user);

        event(new UserRegistered($user, $token));

        return $this->apiBody([
            'user' => UserResource::make($user),
            'token' => $token,
            'token_type' => 'Bearer',
        ])->apiMessage(trans('auth::messages.register.success'))->apiResponse();
    }

    public function verifyOtp(VerifyRegisterOtpRequest $request, VerifyRegisterOtpService $service): JsonResponse
    {
        $service->verfiy($request);

        return $this->apiMessage(trans('auth::messages.register.success'))->apiResponse();
    }

    public function login(LoginRequest $request, LoginAction $action): JsonResponse
    {
        $result = $action->execute(LoginDTO::fromRequest($request->validated()));

        return $this->apiBody([
            'token' => $result['token'],
            'token_type' => 'Bearer',
            'user' => UserResource::make($result['user']->load('groups')),
        ])->apiResponse();
    }

    public function logout(Request $request, LogoutAction $action): JsonResponse
    {
        $action->execute($request->user());

        return $this->apiMessage('Logged out successfully')->apiResponse();
    }

    public function me(Request $request): JsonResponse
    {
        return $this->apiBody([
            'user' => UserResource::make($request->user()->load('addictionProfile', 'groups')),
        ])->apiResponse();
    }

    public function forgotPassword(ForgotPasswordRequest $request, ForgotPasswordAction $action): JsonResponse
    {
        $token = $action->execute($request->mobile_number);
        $user = User::query()->where('mobile_number', $request->mobile_number)->first();

        event(new PasswordResetRequested($user, $token));

        return $this->apiBody(['token' => $token])
            ->apiMessage(trans('messages.password.otp_sent'))
            ->apiResponse();
    }

    public function verifyForgotOtp(VerifyForgotPasswordOtpRequest $request, VerifyForgotPasswordOtpService $service): JsonResponse
    {
        $resetToken = $service->verify($request);

        return $this->apiBody(['reset_token' => $resetToken])
            ->apiMessage(trans('auth::messages.otp.verified'))
            ->apiResponse();
    }

    public function resetPassword(ResetPasswordRequest $request, ResetPasswordAction $action): JsonResponse
    {
        $action->execute($request->reset_token, $request->password);

        return $this->apiMessage(trans('messages.password.reset_success'))->apiResponse();
    }

    public function deleteAccount(Request $request, DeleteAccountAction $action): JsonResponse
    {
        $action->execute($request->user());

        return $this->apiMessage(trans('auth::messages.account.deleted'))->apiResponse();
    }
}
