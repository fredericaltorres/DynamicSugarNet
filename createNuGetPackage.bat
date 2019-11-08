@echo off
cls
echo Create NuGet Package
pause
set PACKAGE_FILE=DynamicSugar2.0.3.0.nupkg
set NUGET_OUTPUT_FOLDER=bin\release\nuGet

if not exist "%NUGET_OUTPUT_FOLDER%" mkdir "%NUGET_OUTPUT_FOLDER%"
if exist "%PACKAGE_FILE%" del "%PACKAGE_FILE%"
.\NuGet.exe pack .\Package.nuspec 

move "%PACKAGE_FILE%" "%NUGET_OUTPUT_FOLDER%"
