<?php

namespace Modules\Groups\Exceptions;

use App\Exceptions\ApiException;
use Symfony\Component\HttpFoundation\Response;

class GroupFullException extends ApiException
{
    public function __construct()
    {
        parent::__construct(
            message: __('The group has reached its maximum capacity.'),
            statusCode: Response::HTTP_UNPROCESSABLE_ENTITY
        );
    }
}
