<?php

namespace App\Exceptions\Sessions;

use Symfony\Component\HttpKernel\Exception\HttpException;

class SessionNotLiveException extends HttpException
{
    public function __construct()
    {
        parent::__construct(422, 'Session is not live.');
    }
}
