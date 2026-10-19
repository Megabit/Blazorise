@echo off
setlocal

rem Build and run all WebAssembly provider demos without a debugger or watch.
rem Builds all demos together. Defaults to DebugNet11 to limit libraries to .NET 11.
rem Optional configuration: Debug, DebugNet11, or Release.
rem Pass -NoBuild to run existing binaries for the chosen configuration without building or restoring.
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0RunDemos.ps1" %*
exit /b %errorlevel%