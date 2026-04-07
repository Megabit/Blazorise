@echo off
setlocal EnableExtensions EnableDelayedExpansion

for %%V in (ProjectDir ProjectName ProjectPath PublishDir PublishProfile WebPublishProfileFile RuntimeIdentifier SelfContained TargetFramework) do set "%%V="

pushd "%~dp0" >nul 2>&1
if errorlevel 1 (
    echo Failed to switch to demos directory.
    exit /b 1
)

set /a projectsWithProfile=0
set /a succeeded=0
set /a failed=0
set /a skipped=0
set /a queued=0
set /a wasmCount=0
set /a serverCount=0

for /r "." %%P in (*.csproj) do (
    set "demoProjectPath=%%~fP"
    set "demoProjectName=%%~nxP"
    set "demoProjectDir=%%~dpP"
    set "demoProfilePath=!demoProjectDir!Properties\PublishProfiles\FolderProfile.pubxml"
    set "demoPublishDir="
    set "demoRuntimeIdentifier="
    set "demoDeleteExistingFiles="

    if exist "!demoProfilePath!" (
        set /a projectsWithProfile+=1

        call :ReadProfileValue "!demoProfilePath!" "PublishDir" demoPublishDir
        if not defined demoPublishDir call :ReadProfileValue "!demoProfilePath!" "PublishUrl" demoPublishDir
        call :ReadProfileValue "!demoProfilePath!" "RuntimeIdentifier" demoRuntimeIdentifier
        call :ReadProfileValue "!demoProfilePath!" "DeleteExistingFiles" demoDeleteExistingFiles

        if not defined demoDeleteExistingFiles set "demoDeleteExistingFiles=false"

        echo.
        if not defined demoPublishDir (
            echo Publish failed: %%~fP
            echo   Reason: PublishUrl/PublishDir not found in FolderProfile.pubxml
            set /a failed+=1
        ) else (
            set /a queued+=1
            set "demoQueuedProject[!queued!]=!demoProjectPath!"
            set "demoQueuedProjectName[!queued!]=!demoProjectName!"
            set "demoQueuedPublishDir[!queued!]=!demoPublishDir!"
            set "demoQueuedDeleteExistingFiles[!queued!]=!demoDeleteExistingFiles!"

            if /i "!demoRuntimeIdentifier!"=="browser-wasm" (
                set /a wasmCount+=1
                set "demoWasmProject[!wasmCount!]=!demoProjectPath!"
                set "demoWasmProjectName[!wasmCount!]=!demoProjectName!"
            ) else (
                set /a serverCount+=1
                set "demoServerProject[!serverCount!]=!demoProjectPath!"
                set "demoServerProjectName[!serverCount!]=!demoProjectName!"
            )
        )
    ) else (
        set /a skipped+=1
        echo Skipping %%~nxP ^(no FolderProfile.pubxml^)
    )
)

echo.
echo Discovery summary:
echo   Projects with profile: !projectsWithProfile!
echo   WASM demos: !wasmCount!
echo   Server demos: !serverCount!
echo   Skipped ^(no profile^): !skipped!

if !projectsWithProfile! equ 0 (
    popd >nul
    exit /b 1
)

if !failed! gtr 0 (
    goto :Summary
)

if !wasmCount! gtr 0 (
    echo.
    echo Prebuilding WASM demo apps...
    call :BuildProjectGroup "WASM demo apps" demoWasmProject demoWasmProjectName !wasmCount!
    if errorlevel 1 (
        set /a failed+=1
        goto :Summary
    )
)

if !serverCount! gtr 0 (
    echo.
    echo Prebuilding server demo apps...
    call :BuildProjectGroup "server demo apps" demoServerProject demoServerProjectName !serverCount!
    if errorlevel 1 (
        set /a failed+=1
        goto :Summary
    )
)

for /L %%I in (1,1,!queued!) do (
    call :PublishProject "!demoQueuedProject[%%I]!" "!demoQueuedProjectName[%%I]!" "!demoQueuedPublishDir[%%I]!" "!demoQueuedDeleteExistingFiles[%%I]!"
    if errorlevel 1 (
        set /a failed+=1
    ) else (
        set /a succeeded+=1
    )
)

:Summary
echo.
echo Publish summary:
echo   Projects with profile: !projectsWithProfile!
echo   Succeeded: !succeeded!
echo   Failed: !failed!
echo   Skipped ^(no profile^): !skipped!

popd >nul

if !failed! gtr 0 (
    exit /b 1
)

exit /b 0

:ReadProfileValue
setlocal DisableDelayedExpansion
set "demoReadProfilePath=%~1"
set "demoReadPropertyName=%~2"
set "demoReadValue="

for /f "tokens=3 delims=<>" %%U in ('findstr /i /c:"<%demoReadPropertyName%>" "%demoReadProfilePath%"') do (
    if not defined demoReadValue set "demoReadValue=%%U"
)

endlocal & set "%~3=%demoReadValue%"
exit /b 0

:BuildProjectGroup
setlocal EnableDelayedExpansion
set "demoGroupName=%~1"
set "demoProjectArray=%~2"
set "demoProjectNameArray=%~3"
set "demoProjectCount=%~4"
set "demoExitCode=0"

for /L %%I in (1,1,!demoProjectCount!) do (
    if !demoExitCode! equ 0 (
        call set "demoLoopProjectPath=%%%demoProjectArray%[%%I]%%"
        call set "demoLoopProjectName=%%%demoProjectNameArray%[%%I]%%"
        echo Restoring !demoLoopProjectName!...
        dotnet restore "!demoLoopProjectPath!"
        if errorlevel 1 set "demoExitCode=1"
    )
)

for /L %%I in (1,1,!demoProjectCount!) do (
    if !demoExitCode! equ 0 (
        call set "demoLoopProjectPath=%%%demoProjectArray%[%%I]%%"
        call set "demoLoopProjectName=%%%demoProjectNameArray%[%%I]%%"
        echo Building !demoLoopProjectName!...
        dotnet build "!demoLoopProjectPath!" -c Release --no-restore
        if errorlevel 1 set "demoExitCode=1"
    )
)

if !demoExitCode! neq 0 echo Build failed: !demoGroupName!
endlocal & exit /b %demoExitCode%

:PublishProject
setlocal DisableDelayedExpansion
set "demoPublishProjectPath=%~1"
set "demoPublishProjectName=%~2"
set "demoTargetPublishDir=%~3"
set "demoDeleteExistingFiles=%~4"

echo.
echo Publishing %demoPublishProjectName% using FolderProfile.pubxml...
echo   Target: %demoTargetPublishDir%

if /i "%demoDeleteExistingFiles%"=="true" (
    call :CleanPublishDir "%demoTargetPublishDir%"
    if errorlevel 1 (
        echo Publish failed: %demoPublishProjectPath%
        endlocal & exit /b 1
    )
)

dotnet publish "%demoPublishProjectPath%" -c Release --no-build --no-restore -p:BuildProjectReferences=false -p:BlazoriseSkipAnalyzerProjectReference=true -p:PublishProfile=FolderProfile -p:PublishUrl="%demoTargetPublishDir%" -p:PublishDir="%demoTargetPublishDir%"

set "demoExitCode=%errorlevel%"
if %demoExitCode% neq 0 echo Publish failed: %demoPublishProjectPath%
endlocal & exit /b %demoExitCode%

:CleanPublishDir
setlocal EnableDelayedExpansion
set "demoCleanPublishDir=%~1"

if not defined demoCleanPublishDir (
    echo   Refusing to clean an empty publish directory.
    endlocal & exit /b 1
)

if "!demoCleanPublishDir:~-1!"=="\" set "demoCleanPublishDir=!demoCleanPublishDir:~0,-1!"

for %%D in ("!demoCleanPublishDir!") do (
    set "demoFullPublishDir=%%~fD"
    set "demoPublishRoot=%%~dD\"
)

if /i "!demoFullPublishDir!"=="!demoPublishRoot!" (
    echo   Refusing to clean drive root: !demoFullPublishDir!
    endlocal & exit /b 1
)

if exist "!demoFullPublishDir!" (
    echo   Cleaning existing files...
    rmdir /s /q "!demoFullPublishDir!"
    if exist "!demoFullPublishDir!" (
        echo   Failed to clean publish directory: !demoFullPublishDir!
        endlocal & exit /b 1
    )
)

mkdir "!demoFullPublishDir!" >nul 2>&1
if not exist "!demoFullPublishDir!" (
    echo   Failed to create publish directory: !demoFullPublishDir!
    endlocal & exit /b 1
)

endlocal & exit /b 0