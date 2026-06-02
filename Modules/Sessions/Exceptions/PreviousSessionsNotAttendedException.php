<?php

namespace Modules\Sessions\Exceptions;

use App\Exceptions\ApiException\ApiException;

class PreviousSessionsNotAttendedException extends ApiException
{
    public function __construct()
    {
        parent::__construct(
            message: __('You must attend previous sessions in order before joining this one.'),
        );
    }

    public function getCustomMessage(): ?string
    {
        return __('You must attend previous sessions in order before joining this one.');
    }

    public function getCustomCode(): int
    {
        return 400;
    }
}
