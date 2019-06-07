cd /d "%~dp0"
set username=%1
set password=%2
"%ProgramFiles(x86)%\\Microsoft Visual Studio\2019\Preview\MSBuild\Current\Bin\MSBuild.exe" Blazorise.sln /p:Configuration=Release /t:Restore;Rebuild /v:m /p:DeployOnBuild=true /p:PublishProfile=IISProfile /p:UserName=%username% /p:Password=%password%
