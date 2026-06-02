<?php

namespace App\Support\Contracts;

use libphonenumber\PhoneNumber;

interface PhoneNumberInterface
{
    public function phoneUtilityObject(): object;

    public function phoneUtility(): ?PhoneNumber;

    public function getNationalNumber(): ?string;
}
