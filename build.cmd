cd /d "%~dp0"
"%ProgramFiles(x86)%\\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe" Blazorise.sln /p:Configuration=Release  /t:Restore;Rebuild /v:m