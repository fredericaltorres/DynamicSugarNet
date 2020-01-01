[CmdletBinding()]
param(
    [Parameter(Mandatory=$false)]
    [Alias('a')]
    [ValidateSet('pack')]
    [string]$action = "pack",
    
    [Parameter(Mandatory=$false)]
    [string]$packageVersion = "0.3.2",

    [Parameter(Mandatory=$false)]
    [string]$configuration = "Release"
)
<#
    https://docs.microsoft.com/en-us/nuget/reference/cli-reference/cli-ref-pack
#>
cd "C:\DVT\DynamicSugarNet"

if($configuration.ToLowerInvariant() -eq "debug") {
    $packageVersion = "$packageVersion-debug"
}

cls
Write-Output "Create NuGet Package - version:$packageVersion, configuration:$configuration"
$NUGET_OUTPUT_FOLDER = "bin\$Configuration\nuGet\"
$PACKAGE_FILE = "$NUGET_OUTPUT_FOLDER\DynamicSugar2.$packageVersion.nupkg"

if(Test-Path $PACKAGE_FILE) {
    Remove-Item $PACKAGE_FILE 
}

& .\NuGet.exe pack .\Package.nuspec -Version $packageVersion -Properties Configuration=$Configuration -OutputDirectory "$NUGET_OUTPUT_FOLDER"
