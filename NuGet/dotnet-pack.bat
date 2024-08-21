del *.nupkg
del *.snupkg

dotnet pack ../Source/Blazorise/Blazorise.csproj -o . -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg

dotnet pack ../Source/Blazorise.AntDesign/Blazorise.AntDesign.csproj -o . -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
dotnet pack ../Source/Blazorise.Bootstrap/Blazorise.Bootstrap.csproj -o . -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
dotnet pack ../Source/Blazorise.Bootstrap5/Blazorise.Bootstrap5.csproj -o . -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
dotnet pack ../Source/Blazorise.Bulma/Blazorise.Bulma.csproj -o . -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
dotnet pack ../Source/Blazorise.Material/Blazorise.Material.csproj -o . -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
dotnet pack ../Source/Blazorise.Tailwind/Blazorise.Tailwind.csproj -o . -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
dotnet pack ../Source/Blazorise.FluentUI2/Blazorise.FluentUI2.csproj -o . -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg

dotnet pack ../Source/Extensions/Blazorise.Animate/Blazorise.Animate.csproj -o . -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
dotnet pack ../Source/Extensions/Blazorise.Captcha/Blazorise.Captcha.csproj -o . -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
dotnet pack ../Source/Extensions/Blazorise.Captcha.ReCaptcha/Blazorise.Captcha.ReCaptcha.csproj -o . -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
dotnet pack ../Source/Extensions/Blazorise.Charts/Blazorise.Charts.csproj -o . -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
dotnet pack ../Source/Extensions/Blazorise.Charts.Annotation/Blazorise.Charts.Annotation.csproj -o . -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
dotnet pack ../Source/Extensions/Blazorise.Charts.DataLabels/Blazorise.Charts.DataLabels.csproj -o . -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
dotnet pack ../Source/Extensions/Blazorise.Charts.Streaming/Blazorise.Charts.Streaming.csproj -o . -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
dotnet pack ../Source/Extensions/Blazorise.Charts.Trendline/Blazorise.Charts.Trendline.csproj -o . -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
dotnet pack ../Source/Extensions/Blazorise.Charts.Zoom/Blazorise.Charts.Zoom.csproj -o . -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
dotnet pack ../Source/Extensions/Blazorise.Components/Blazorise.Components.csproj -o . -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
dotnet pack ../Source/Extensions/Blazorise.Cropper/Blazorise.Cropper.csproj -o . -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
dotnet pack ../Source/Extensions/Blazorise.DataGrid/Blazorise.DataGrid.csproj -o . -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
dotnet pack ../Source/Extensions/Blazorise.FluentValidation/Blazorise.FluentValidation.csproj -o . -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
dotnet pack ../Source/Extensions/Blazorise.LoadingIndicator/Blazorise.LoadingIndicator.csproj -o . -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
dotnet pack ../Source/Extensions/Blazorise.LottieAnimation/Blazorise.LottieAnimation.csproj -o . -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
dotnet pack ../Source/Extensions/Blazorise.Markdown/Blazorise.Markdown.csproj -o . -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
dotnet pack ../Source/Extensions/Blazorise.QRCode/Blazorise.QRCode.csproj -o . -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
dotnet pack ../Source/Extensions/Blazorise.RichTextEdit/Blazorise.RichTextEdit.csproj -o . -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
dotnet pack ../Source/Extensions/Blazorise.Sidebar/Blazorise.Sidebar.csproj -o . -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
dotnet pack ../Source/Extensions/Blazorise.SignaturePad/Blazorise.SignaturePad.csproj -o . -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
dotnet pack ../Source/Extensions/Blazorise.Snackbar/Blazorise.Snackbar.csproj -o . -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
dotnet pack ../Source/Extensions/Blazorise.SpinKit/Blazorise.SpinKit.csproj -o . -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
dotnet pack ../Source/Extensions/Blazorise.Splitter/Blazorise.Splitter.csproj -o . -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
dotnet pack ../Source/Extensions/Blazorise.TreeView/Blazorise.TreeView.csproj -o . -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
dotnet pack ../Source/Extensions/Blazorise.Video/Blazorise.Video.csproj -o . -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg

dotnet pack ../Source/Extensions/Blazorise.Icons.Bootstrap/Blazorise.Icons.Bootstrap.csproj -o . -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
dotnet pack ../Source/Extensions/Blazorise.Icons.FontAwesome/Blazorise.Icons.FontAwesome.csproj -o . -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
dotnet pack ../Source/Extensions/Blazorise.Icons.Material/Blazorise.Icons.Material.csproj -o . -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
dotnet pack ../Source/Extensions/Blazorise.Icons.FluentUI/Blazorise.Icons.FluentUI.csproj -o . -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg

dotnet pack ../Source/Helpers/Blazorise.Tests.bUnit/Blazorise.Tests.bUnit.csproj -o . -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg

dotnet pack ../Source/SourceGenerators/Blazorise.Generator.Features/Blazorise.Generator.Features.csproj -o . -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg