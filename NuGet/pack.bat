cd ..
call clean.cmd
call build.cmd
cd NuGet

del *.nupkg
del *.snupkg

nuget pack Blazorise.nuspec
nuget pack Blazorise.Bootstrap.nuspec
nuget pack Blazorise.Material.nuspec
nuget pack Blazorise.Bulma.nuspec
nuget pack Blazorise.AntDesign.nuspec

nuget pack Blazorise.Charts.nuspec
nuget pack Blazorise.Charts.Streaming.nuspec
nuget pack Blazorise.DataGrid.nuspec
nuget pack Blazorise.Sidebar.nuspec
nuget pack Blazorise.Snackbar.nuspec
nuget pack Blazorise.Components.nuspec

nuget pack Blazorise.Icons.FontAwesome.nuspec
nuget pack Blazorise.Icons.Material.nuspec