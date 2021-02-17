del *.nupkg

dotnet pack ../Source/Blazorise/Blazorise.csproj --force -o . -c Release
dotnet pack ../Source/Blazorise.Bootstrap/Blazorise.Bootstrap.csproj --force -o . -c Release
dotnet pack ../Source/Blazorise.Material/Blazorise.Material.csproj --force -o . -c Release
dotnet pack ../Source/Blazorise.Bulma/Blazorise.Bulma.csproj --force -o . -c Release
dotnet pack ../Source/Blazorise.AntDesign/Blazorise.AntDesign.csproj --force -o . -c Release
dotnet pack ../Source/Blazorise.Frolic/Blazorise.Frolic.csproj --force -o . -c Release

dotnet pack ../Source/Extensions/Blazorise.Animate/Blazorise.Animate.csproj --force -o . -c Release
dotnet pack ../Source/Extensions/Blazorise.Charts/Blazorise.Charts.csproj --force -o . -c Release
dotnet pack ../Source/Extensions/Blazorise.Charts.Streaming/Blazorise.Charts.Streaming.csproj --force -o . -c Release
dotnet pack ../Source/Extensions/Blazorise.DataGrid/Blazorise.DataGrid.csproj --force -o . -c Release
dotnet pack ../Source/Extensions/Blazorise.Sidebar/Blazorise.Sidebar.csproj --force -o . -c Release
dotnet pack ../Source/Extensions/Blazorise.Snackbar/Blazorise.Snackbar.csproj --force -o . -c Release
dotnet pack ../Source/Extensions/Blazorise.Components/Blazorise.Components.csproj --force -o . -c Release
dotnet pack ../Source/Extensions/Blazorise.TreeView/Blazorise.TreeView.csproj --force -o . -c Release
dotnet pack ../Source/Extensions/Blazorise.RichTextEdit/Blazorise.RichTextEdit.csproj --force -o . -c Release

dotnet pack ../Source/Extensions/Blazorise.Icons.FontAwesome/Blazorise.Icons.FontAwesome.csproj --force -o . -c Release
dotnet pack ../Source/Extensions/Blazorise.Icons.Material/Blazorise.Icons.Material.csproj --force -o . -c Release