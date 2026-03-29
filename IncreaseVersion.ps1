# Update-Version.ps1
# Increments the third segment of AssemblyVersion, FileVersion, and VersionPrefix
# in fAI.csproj located in the current directory.

$projectFile =  "C:\DVT\DynamicSugarNet\DynamicSugarStandard\DynamicSugarStandard.csproj"

if (-not (Test-Path $projectFile)) {
    Write-Error "Project file not found: $projectFile"
    exit 1
}

$content = Get-Content $projectFile -Raw

$tags = @("AssemblyVersion", "FileVersion", "VersionPrefix")
$anyUpdated = $false

foreach ($tag in $tags) {
    $pattern = "(<$tag>)(\d+)\.(\d+)\.(\d+)\.(\d+)(</$tag>)"

    if ($content -match $pattern) {
        $oldVersion = "$($Matches[2]).$($Matches[3]).$($Matches[4]).$($Matches[5])"
        $newThird   = [int]$Matches[4] + 1
        $newVersion = "$($Matches[2]).$($Matches[3]).$newThird.$($Matches[5])"

        $content = $content -replace $pattern, "`${1}$newVersion`${6}"

        Write-Host "[$tag]  $oldVersion  -->  $newVersion"
        $anyUpdated = $true
    } else {
        Write-Warning "Tag <$tag> with a 4-part version was not found � skipped."
    }
}

if ($anyUpdated) {
    Set-Content $projectFile $content -NoNewline
    Write-Host "`nfAI.csproj updated successfully."
} else {
    Write-Host "`nNo version tags were updated."
}