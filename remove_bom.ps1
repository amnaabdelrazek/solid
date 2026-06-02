$files = Get-ChildItem -Path . -Recurse -Filter *.php | Where-Object { $_.FullName -notmatch "node_modules|vendor" }

foreach ($file in $files) {
    $bytes = [System.IO.File]::ReadAllBytes($file.FullName)
    if ($bytes.Length -ge 3 -and $bytes[0] -eq 0xEF -and $bytes[1] -eq 0xBB -and $bytes[2] -eq 0xBF) {
        $newBytes = New-Object byte[] ($bytes.Length - 3)
        [System.Array]::Copy($bytes, 3, $newBytes, 0, $newBytes.Length)
        [System.IO.File]::WriteAllBytes($file.FullName, $newBytes)
        Write-Host "Removed BOM from $($file.FullName)"
    }
}
