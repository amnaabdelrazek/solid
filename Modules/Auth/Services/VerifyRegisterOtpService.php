<?php

namespace Modules\Auth\Services;

use Illuminate\Cache\CacheManager;
use Illuminate\Foundation\Application;
use Modules\Auth\Contracts\VerifyOtpRequestContract;
use Modules\Auth\Exceptions\OtpException;
use Modules\User\Models\User;
use Modules\User\Repositories\UserRepositoryInterface;

class VerifyRegisterOtpService
{
    public function __construct(private UserRepositoryInterface $repository) {}

    public function verfiy(VerifyOtpRequestContract $request): bool
    {
        $user = $this->getUser($request);
        $this->validateOtp($request);
        $this->repository->updateActive($user, true);

        return true;
    }

    public function getTokenFromRequest(VerifyOtpRequestContract $request): ?string
    {
        return $request->getToken();
    }

    /**
     * @param  string  $token
     */
    public function getUserId(VerifyOtpRequestContract $request): string|int
    {
        return explode('|', $this->getTokenFromRequest($request) ?? '')[0] ?? 0;
    }

    /**
     * @param  int|string  $user_id
     */
    public function getUser($request): User
    {
        return $this->repository->findOrFail($this->getUserId($request));

    }

    /**
     * @param  $token
     * @return bool|CacheManager|Application|mixed|object
     */
    public function getOtpFromCash($request): mixed
    {
        return cache("otp:{$this->getUserId($request)}");
    }

    /**
     * @throws \Throwable
     */
    public function validateOtp(VerifyOtpRequestContract $request): void
    {
        $otp = $this->getOtpFromCash($request);
        throw_if(! $otp, OtpException::expired());
        throw_if($otp != $request->getOtp(), OtpException::invalid());
    }
}
