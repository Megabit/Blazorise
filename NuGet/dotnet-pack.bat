del *.nupkg

dotnet pack ../Source/Blazorise/Blazorise.csproj -o . -c Release
dotnet pack ../Source/Blazorise.Bootstrap/Blazorise.Bootstrap.csproj -o . -c Release
dotnet pack ../Source/Blazorise.Material/Blazorise.Material.csproj -o . -c Release
dotnet pack ../Source/Blazorise.Bulma/Blazorise.Bulma.csproj -o . -c Release
dotnet pack ../Source/Blazorise.Frolic/Blazorise.Frolic.csproj -o . -c Release

dotnet pack ../Source/Extensions/Blazorise.Charts/Blazorise.Charts.csproj -o . -c Release
dotnet pack ../Source/Extensions/Blazorise.DataGrid/Blazorise.DataGrid.csproj -o . -c Release
dotnet pack ../Source/Extensions/Blazorise.Sidebar/Blazorise.Sidebar.csproj -o . -c Release
dotnet pack ../Source/Extensions/Blazorise.Snackbar/Blazorise.Snackbar.csproj -o . -c Release
dotnet pack ../Source/Extensions/Blazorise.Components/Blazorise.Components.csproj -o . -c Release

dotnet pack ../Source/Extensions/Blazorise.Icons.FontAwesome/Blazorise.Icons.FontAwesome.csproj -o . -c Release
dotnet pack ../Source/Extensions/Blazorise.Icons.Material/Blazorise.Icons.Material.csproj -o . -c Release