@echo off
setlocal enabledelayedexpansion

echo ==========================================
echo  Publishing Lifter.Maui to NuGet
echo ==========================================

if "%1"=="" (
    echo Error: Please provide your NuGet API Key as the first argument.
    echo Usage: publish-maui.bat ^<API_KEY^>
    exit /b 1
)

set API_KEY=%1
set PACKAGE_DIR=nupkgs

if not exist "%PACKAGE_DIR%" mkdir "%PACKAGE_DIR%"

echo.
echo [1/3] Restoring Lifter.Maui...
dotnet restore Lifter.Maui\Lifter.Maui.csproj
if %ERRORLEVEL% neq 0 (
    echo Error restoring Lifter.Maui
    exit /b 1
)

echo.
echo [2/3] Packing Lifter.Maui...
dotnet pack Lifter.Maui\Lifter.Maui.csproj --configuration Release --output "%PACKAGE_DIR%" --no-restore
if %ERRORLEVEL% neq 0 (
    echo Error packing Lifter.Maui
    exit /b 1
)

echo.
echo [3/3] Pushing to NuGet...
for %%f in ("%PACKAGE_DIR%\Lifter.Maui.*.nupkg") do (
    echo Pushing %%f...
    dotnet nuget push "%%f" --api-key "%API_KEY%" --source https://api.nuget.org/v3/index.json --skip-duplicate
)

echo.
echo ==========================================
echo  DONE! Lifter.Maui published successfully.
echo ==========================================
