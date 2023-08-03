: This is here to enable console colors when using watch
: Usefull for debugging when developping
set DOTNET_WATCH_SUPPRESS_LAUNCH_BROWSER=1
title Blazorise.Demo.AntDesign
cd /d %~dp0
dotnet watch run 
