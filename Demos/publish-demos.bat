@echo off
setlocal EnableExtensions EnableDelayedExpansion

pushd "%~dp0" >nul 2>&1
if errorlevel 1 (
    echo Failed to switch to repository root.
    exit /b 1
)

set /a projectsWithProfile=0
set /a succeeded=0
set /a failed=0
set /a skipped=0

for /r "." %%P in (*.csproj) do (
    set "projectPath=%%~fP"
    set "projectDir=%%~dpP"
    set "profilePath=!projectDir!Properties\PublishProfiles\FolderProfile.pubxml"
    set "publishDir="

    if exist "!profilePath!" (
        set /a projectsWithProfile+=1

        for /f "tokens=3 delims=<>" %%U in ('findstr /i /c:"<PublishDir>" "!profilePath!"') do (
            if not defined publishDir set "publishDir=%%U"
        )

        if not defined publishDir (
            for /f "tokens=3 delims=<>" %%U in ('findstr /i /c:"<PublishUrl>" "!profilePath!"') do (
                if not defined publishDir set "publishDir=%%U"
            )
        )

        echo.
        if not defined publishDir (
            echo Publish failed: %%~fP
            echo   Reason: PublishUrl/PublishDir not found in FolderProfile.pubxml
            set /a failed+=1
        ) else (
            echo Publishing %%~nxP using FolderProfile.pubxml...
            echo   Target: !publishDir!
            dotnet publish "!projectPath!" -c Release -p:PublishProfile=FolderProfile -p:PublishDir="!publishDir!"
            if errorlevel 1 (
                echo Publish failed: %%~fP
                set /a failed+=1
            ) else (
                echo Publish succeeded: %%~fP
                set /a succeeded+=1
            )
        )
    ) else (
        set /a skipped+=1
        echo Skipping %%~nxP ^(no FolderProfile.pubxml^)
    )
)

echo.
echo Publish summary:
echo   Projects with profile: !projectsWithProfile!
echo   Succeeded: !succeeded!
echo   Failed: !failed!
echo   Skipped ^(no profile^): !skipped!

popd >nul

if !projectsWithProfile! equ 0 (
    exit /b 1
)

if !failed! gtr 0 (
    exit /b 1
)

exit /b 0