$replacements = @{
    "App\\Models\\User" = "Modules\User\Models\User"
    "App\\Models\\AddictionProfile" = "Modules\User\Models\AddictionProfile"
    "App\\Models\\Substance" = "Modules\User\Models\Substance"
    "App\\Models\\SubstanceCategory" = "Modules\User\Models\SubstanceCategory"
    "App\\Models\\Group" = "Modules\Groups\Models\Group"
    "App\\Models\\Payment" = "Modules\Payments\Models\Payment"
    "App\\Models\\Recommendation" = "Modules\Recommendations\Models\Recommendation"
    "App\\Models\\LookupType" = "Modules\Lookups\Models\LookupType"
    "App\\Models\\LookupValue" = "Modules\Lookups\Models\LookupValue"
    "App\\Models\\Session" = "Modules\Sessions\Models\Session"
    "App\\Models\\SessionAttendance" = "Modules\Sessions\Models\SessionAttendance"
    "App\\Models\\BreakoutRoom" = "Modules\Sessions\Models\BreakoutRoom"
    "App\\Models\\DeviceSession" = "Modules\Auth\Models\DeviceSession"
    "Modules\\\\User\\\\Models\\\\User" = "Modules\User\Models\User"
    "Modules\\\\User\\\\Models\\\\AddictionProfile" = "Modules\User\Models\AddictionProfile"
    "Modules\\\\User\\\\Models\\\\Substance" = "Modules\User\Models\Substance"
    "Modules\\\\User\\\\Models\\\\SubstanceCategory" = "Modules\User\Models\SubstanceCategory"
    "Modules\\\\Groups\\\\Models\\\\Group" = "Modules\Groups\Models\Group"
    "Modules\\\\Payments\\\\Models\\\\Payment" = "Modules\Payments\Models\Payment"
    "Modules\\\\Recommendations\\\\Models\\\\Recommendation" = "Modules\Recommendations\Models\Recommendation"
    "Modules\\\\Lookups\\\\Models\\\\LookupType" = "Modules\Lookups\Models\LookupType"
    "Modules\\\\Lookups\\\\Models\\\\LookupValue" = "Modules\Lookups\Models\LookupValue"
    "Modules\\\\Sessions\\\\Models\\\\Session" = "Modules\Sessions\Models\Session"
    "Modules\\\\Sessions\\\\Models\\\\SessionAttendance" = "Modules\Sessions\Models\SessionAttendance"
    "Modules\\\\Sessions\\\\Models\\\\BreakoutRoom" = "Modules\Sessions\Models\BreakoutRoom"
    "Modules\\\\Auth\\\\Models\\\\DeviceSession" = "Modules\Auth\Models\DeviceSession"
}

$files = Get-ChildItem -Path . -Recurse -Filter *.php | Where-Object { $_.FullName -notmatch "node_modules|vendor" }

foreach ($file in $files) {
    if ($file.FullName -match "replace_models.ps1") { continue }
    
    $content = Get-Content -Path $file.FullName -Raw
    $modified = $false
    
    foreach ($key in $replacements.Keys) {
        if ($content -match $key) {
            $content = $content -replace $key, $replacements[$key]
            $modified = $true
        }
    }
    
    if ($modified) {
        Set-Content -Path $file.FullName -Value $content -Encoding UTF8
        Write-Host "Updated $($file.FullName)"
    }
}
