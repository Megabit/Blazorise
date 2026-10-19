@echo off
setlocal

rem Run all WebAssembly provider demos with watch, without opening browsers.
rem Optional argument: Debug, DebugNet11, or Release. Defaults to DebugNet11.
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0RunDemos.ps1" -Watch %*
exit /b %errorlevel%