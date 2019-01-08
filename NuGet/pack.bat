rem cd ..
rem call build.cmd
rem cd NuGet

del *.nupkg

nuget pack Blazorise.nuspec
nuget pack Blazorise.Bootstrap.nuspec

rem nuget pack ..\Source\Blazorise\Blazorise.csproj
rem nuget pack ..\Source\Blazorise.Bootstrap\Blazorise.Bootstrap.csproj
rem nuget pack ..\Source\Blazorise.Bulma\Blazorise.Bulma.csproj
rem nuget pack ..\Source\Blazorise.Material\Blazorise.Material.csproj

rem nuget pack ..\Source\Extensions\Blazorise.Charts\Blazorise.Charts.csproj
rem nuget pack ..\Source\Extensions\Blazorise.Sidebar\Blazorise.Sidebar.csproj
rem nuget pack ..\Source\Extensions\Blazorise.Snackbar\Blazorise.Snackbar.csproj