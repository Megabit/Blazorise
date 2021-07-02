del *.nupkg
del *.snupkg

dotnet pack ../Source/Blazorise/Blazorise.csproj -o . -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
dotnet pack ../Source/Blazorise.Bootstrap/Blazorise.Bootstrap.csproj -o . -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
dotnet pack ../Source/Blazorise.Material/Blazorise.Material.csproj -o . -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
dotnet pack ../Source/Blazorise.Bulma/Blazorise.Bulma.csproj -o . -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
dotnet pack ../Source/Blazorise.AntDesign/Blazorise.AntDesign.csproj -o . -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg

dotnet pack ../Source/Extensions/Blazorise.Animate/Blazorise.Animate.csproj -o . -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
dotnet pack ../Source/Extensions/Blazorise.Charts/Blazorise.Charts.csproj -o . -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
dotnet pack ../Source/Extensions/Blazorise.Charts.Streaming/Blazorise.Charts.Streaming.csproj -o . -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
dotnet pack ../Source/Extensions/Blazorise.DataGrid/Blazorise.DataGrid.csproj -o . -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
dotnet pack ../Source/Extensions/Blazorise.Sidebar/Blazorise.Sidebar.csproj -o . -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
dotnet pack ../Source/Extensions/Blazorise.Snackbar/Blazorise.Snackbar.csproj -o . -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
dotnet pack ../Source/Extensions/Blazorise.Components/Blazorise.Components.csproj -o . -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
dotnet pack ../Source/Extensions/Blazorise.TreeView/Blazorise.TreeView.csproj -o . -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
dotnet pack ../Source/Extensions/Blazorise.RichTextEdit/Blazorise.RichTextEdit.csproj -o . -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
dotnet pack ../Source/Extensions/Blazorise.SpinKit/Blazorise.SpinKit.csproj -o . -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
dotnet pack ../Source/Extensions/Blazorise.Markdown/Blazorise.Markdown.csproj -o . -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg

dotnet pack ../Source/Extensions/Blazorise.Icons.FontAwesome/Blazorise.Icons.FontAwesome.csproj -o . -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
dotnet pack ../Source/Extensions/Blazorise.Icons.Material/Blazorise.Icons.Material.csproj -o . -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg