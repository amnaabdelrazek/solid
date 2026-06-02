<?php

namespace App\Enums;

enum RecommendationType: string
{
    case PHARMACY = 'pharmacy';
    case CLINIC = 'clinic';
    case TREATMENT_CTR = 'treatment_center';
}
