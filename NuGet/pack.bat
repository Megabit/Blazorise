cd ..
call build.cmd
cd NuGet

del *.nupkg

nuget pack ..\Source\Blazorise\Blazorise.csproj
nuget pack ..\Source\Blazorise.Bootstrap\Blazorise.Bootstrap.csproj
nuget pack ..\Source\Blazorise.Bulma\Blazorise.Bulma.csproj
nuget pack ..\Source\Blazorise.Material\Blazorise.Material.csproj
nuget pack ..\Source\Blazorise.Charts\Blazorise.Charts.csproj
nuget pack ..\Source\Blazorise.Sidebar\Blazorise.Sidebar.csproj