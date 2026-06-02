<?php

namespace App\Exceptions\Sessions;

use Symfony\Component\HttpKernel\Exception\HttpException;

class UnauthorizedGroupAccessException extends HttpException
{
    public function __construct()
    {
        parent::__construct(403, 'You are not a member of this group.');
    }
}
