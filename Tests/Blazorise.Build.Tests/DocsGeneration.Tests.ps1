param(
    [string]$TestRoot = (Join-Path ([System.IO.Path]::GetTempPath()) ("Blazorise.DocsGeneration." + [guid]::NewGuid().ToString('N')))
)

# Run explicitly with PowerShell 7 and the .NET 11 SDK. Outputs remain available for inspection.
$ErrorActionPreference = 'Stop'
$repoRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$docsRoot = Join-Path $repoRoot 'Documentation/Blazorise.Docs'
$compilerProject = Join-Path $repoRoot 'Documentation/Blazorise.Docs.Compiler/Blazorise.Docs.Compiler.csproj'
$TestRoot = [System.IO.Path]::GetFullPath($TestRoot)
if (Test-Path -LiteralPath $TestRoot) {
    throw "Use a new test directory: $TestRoot"
}
$generatedRoot = Join-Path $TestRoot 'generated'
$examplePath = 'Pages/Docs/Code/AntDesignScriptsExampleCode.html'

function Invoke-DotNet {
    & dotnet @args
    if ($LASTEXITCODE -ne 0) {
        throw "dotnet failed: $args"
    }
}

function Get-SourceSnapshot {
    $snapshot = @{}
    Get-ChildItem -LiteralPath $docsRoot -Recurse -File | Where-Object {
        $_.FullName -notmatch '[/\\](obj|bin)[/\\]' -and
        ($_.Extension -eq '.html' -or $_.Name -in @('Snippets.generated.cs', 'docs-index.json', 'docs-api-index.json', 'NewFilesToBuild.txt') -or $_.Name.EndsWith('.ApiDocs.cs'))
    } | ForEach-Object {
        $snapshot[$_.FullName] = (Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash
    }
    return $snapshot
}

function Assert-Version([string]$Text, [string]$Version) {
    if (!$Text.Contains("?v=$Version") -or $Text.Contains('__BLAZORISE_VERSION__')) {
        throw "Expected resolved asset version $Version."
    }
}

$before = Get-SourceSnapshot
Push-Location $repoRoot
try {
    foreach ($case in @(
        @{ Package = '2.3.3'; Asset = '2.3.3.0' },
        @{ Package = '2.3.3.1'; Asset = '2.3.3.1' }
    )) {
        Invoke-DotNet run --configuration Debug "--property:BlazoriseVersion=$($case.Package)" --project $compilerProject -- --output-path $generatedRoot

        Assert-Version ([System.IO.File]::ReadAllText((Join-Path $generatedRoot $examplePath))) $case.Asset
        Assert-Version ([System.IO.File]::ReadAllText((Join-Path $generatedRoot 'Models/Snippets.generated.cs'))) $case.Asset
        Assert-Version ([System.IO.File]::ReadAllText((Join-Path $generatedRoot 'Resources/docs-index.json'))) $case.Asset

        $packageHtml = [System.IO.File]::ReadAllText((Join-Path $generatedRoot 'Pages/Docs/Code/AnalyzerPackageReferenceExampleCode.html'))
        $packageExample = [System.Net.WebUtility]::HtmlDecode([regex]::Replace($packageHtml, '<[^>]*>', ''))
        $expectedPackageAttribute = 'Version="' + $case.Package + '"'
        if (!$packageExample.Contains($expectedPackageAttribute) -or $packageExample.Contains('__BLAZORISE_PACKAGE_VERSION__')) {
            throw "Expected the exact NuGet version $($case.Package) in the package reference example."
        }

        if (!(Get-Content (Join-Path $generatedRoot 'ExampleCodeFiles.txt')).Contains($examplePath)) {
            throw 'The example is missing from the generated resource manifest.'
        }
    }

    # Exercise the production resource-inclusion target in a small project. API documentation
    # compilation belongs to the full docs build; this fixture checks HTML and snippet constants.
    [xml]$docsProject = Get-Content (Join-Path $docsRoot 'Blazorise.Docs.csproj') -Raw
    $resourceTarget = $docsProject.SelectSingleNode('/Project/Target[@Name="IncludeGeneratedDocs"]').OuterXml
    $generationTargets = [System.Security.SecurityElement]::Escape((Join-Path $repoRoot 'Build/Blazorise.Docs.Generation.targets'))
    $generatedPathXml = [System.Security.SecurityElement]::Escape($generatedRoot)
    $fixtureProject = Join-Path $TestRoot 'DocsResources.csproj'
    [System.IO.File]::WriteAllText($fixtureProject, @"
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net11.0</TargetFramework>
    <OutputType>Exe</OutputType>
    <RootNamespace>Blazorise.Docs</RootNamespace>
    <EnableDefaultCompileItems>false</EnableDefaultCompileItems>
    <GenerateDocs>false</GenerateDocs>
    <DocsGeneratedOutputPath>$generatedPathXml</DocsGeneratedOutputPath>
    <ApiDocsIntermediatePath>$generatedPathXml/UnusedApiDocs</ApiDocsIntermediatePath>
  </PropertyGroup>
  <ItemGroup>
    <Compile Include="Program.cs" />
  </ItemGroup>
  <Import Project="$generationTargets" />
  $resourceTarget
</Project>
"@)
    # Run the checks on .NET 11 independently of PowerShell's own runtime.
    [System.IO.File]::WriteAllText((Join-Path $TestRoot 'Program.cs'), @'
#region Using directives
using System;
using System.IO;
using System.Reflection;
using Blazorise.Docs.Models;
#endregion

var resourceName = args[0];
var assembly = Assembly.GetExecutingAssembly();
using ( var stream = assembly.GetManifestResourceStream( resourceName ) )
{
    if ( stream is null )
    {
        throw new InvalidOperationException( $"Missing embedded resource: {resourceName}" );
    }

    using ( var reader = new StreamReader( stream ) )
    {
        AssertVersion( reader.ReadToEnd(), "2.3.3.1" );
    }
}

AssertVersion( Snippets.AntDesignScriptsExample, "2.3.3.1" );

var packageSnippet = Snippets.AnalyzerPackageReferenceExample;
if ( !packageSnippet.Contains( "Version=\"2.3.3.1\"" ) || packageSnippet.Contains( "__BLAZORISE_PACKAGE_VERSION__" ) )
{
    throw new InvalidOperationException( "The copyable package reference does not contain the resolved NuGet version." );
}

static void AssertVersion( string text, string version )
{
    if ( !text.Contains( $"?v={version}" ) || text.Contains( "__BLAZORISE_VERSION__" ) )
    {
        throw new InvalidOperationException( $"Expected resolved asset version {version}." );
    }
}
'@)
    Invoke-DotNet build $fixtureProject --configuration Debug --nologo
    $resourceName = 'Blazorise.Docs.' + $examplePath.Replace('/', '.')
    Invoke-DotNet run --project $fixtureProject --configuration Debug --no-build -- $resourceName

    $after = Get-SourceSnapshot
    if ($before.Count -ne $after.Count) {
        throw 'Documentation generation added or removed source-tree outputs.'
    }
    foreach ($path in $before.Keys) {
        if ($before[$path] -cne $after[$path]) {
            throw "Documentation generation changed a source-tree output: $path"
        }
    }

    Write-Host "Documentation generation checks passed. Outputs: $TestRoot"
}
finally {
    Pop-Location
}