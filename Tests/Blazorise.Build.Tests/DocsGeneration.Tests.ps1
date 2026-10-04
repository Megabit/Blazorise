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

function Assert-SearchIndexesMatch([string]$OutputRoot) {
    foreach ($fileName in @('docs-index.json', 'docs-api-index.json')) {
        $fullIndex = Get-Content -LiteralPath (Join-Path $generatedRoot "Resources/$fileName") -Raw | ConvertFrom-Json
        $searchDataIndex = Get-Content -LiteralPath (Join-Path $OutputRoot "Resources/$fileName") -Raw | ConvertFrom-Json
        $fullIndex.PSObject.Properties.Remove('generatedUtc')
        $searchDataIndex.PSObject.Properties.Remove('generatedUtc')
        if (($fullIndex | ConvertTo-Json -Depth 100 -Compress) -cne ($searchDataIndex | ConvertTo-Json -Depth 100 -Compress)) {
            throw "Search data generation produced different content: $fileName"
        }
    }
}

$before = Get-SourceSnapshot
Push-Location $repoRoot
try {
    foreach ($case in @(
        @{ Package = '2.3.3'; Asset = '2.3.3.0' },
        @{ Package = '2.3.3.1'; Asset = '2.3.3.1' }
    )) {
        $apiOutputPath = Join-Path $generatedRoot 'ApiDocs'
        $apiSnapshot = @{}
        $staleApiPath = Join-Path $apiOutputPath 'RemovedComponent.ApiDocs.cs'
        if (Test-Path -LiteralPath $apiOutputPath) {
            foreach ($file in Get-ChildItem -LiteralPath $apiOutputPath -Filter '*.ApiDocs.cs' -File) {
                $apiSnapshot[$file.FullName] = @{
                    Hash = (Get-FileHash -LiteralPath $file.FullName -Algorithm SHA256).Hash
                    Timestamp = $file.LastWriteTimeUtc
                }
            }
            [System.IO.File]::WriteAllText($staleApiPath, '// Stale output from a removed component library.')
        }

        Invoke-DotNet run --configuration Debug "--property:BlazoriseVersion=$($case.Package)" --project $compilerProject -- --output-path $generatedRoot

        if (!(Test-Path -LiteralPath (Join-Path $apiOutputPath 'Blazorise.ApiDocs.cs'))) {
            throw 'Full generation did not produce API documentation source.'
        }
        if (Test-Path -LiteralPath $staleApiPath) {
            throw 'Generation did not remove stale API documentation source.'
        }
        foreach ($path in $apiSnapshot.Keys) {
            if ($apiSnapshot[$path].Hash -cne (Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash) {
                throw "Changing only the package version changed API documentation: $path"
            }
            if ($apiSnapshot[$path].Timestamp -ne [System.IO.File]::GetLastWriteTimeUtc($path)) {
                throw "Unchanged API documentation was rewritten: $path"
            }
        }

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

    $unchangedOutputs = @{}
    foreach ($file in Get-ChildItem -LiteralPath $generatedRoot -Recurse -File) {
        $unchangedOutputs[$file.FullName] = $file.LastWriteTimeUtc
    }
    $cachedLog = @(Invoke-DotNet run --configuration Debug --no-build --project $compilerProject -- --output-path $generatedRoot)
    $cachedLog | ForEach-Object { Write-Host $_ }
    foreach ($stage in @('examples', 'api', 'search')) {
        if (!($cachedLog -like "*${stage} is up to date.*")) {
            throw "Unchanged generation did not reuse the $stage cache."
        }
    }
    foreach ($path in $unchangedOutputs.Keys) {
        if ($unchangedOutputs[$path] -ne [System.IO.File]::GetLastWriteTimeUtc($path)) {
            throw "Unchanged generation rewrote an output: $path"
        }
    }

    # Missing files must be repaired even when all inputs are unchanged.
    $missingExample = Join-Path $generatedRoot $examplePath
    $missingApi = Join-Path $generatedRoot 'ApiDocs/Blazorise.ApiDocs.cs'
    [System.IO.File]::Delete($missingExample)
    [System.IO.File]::Delete($missingApi)
    Invoke-DotNet run --configuration Debug --no-build --project $compilerProject -- --output-path $generatedRoot
    Assert-Version ([System.IO.File]::ReadAllText($missingExample)) '2.3.3.1'
    if (!(Test-Path -LiteralPath $missingApi)) {
        throw 'Cached generation did not restore a missing API documentation file.'
    }

    # MCP needs the same indexes as full generation, without HTML or C# outputs.
    $searchDataRoot = Join-Path $TestRoot 'search-data-only'
    Invoke-DotNet run --configuration Debug --no-build --project $compilerProject -- --output-path $searchDataRoot --search-data-only true
    Assert-SearchIndexesMatch $searchDataRoot
    if (@(Get-ChildItem -LiteralPath $searchDataRoot -Recurse -File | Where-Object { $_.FullName -notmatch '[/\\]\.cache[/\\]' }).Count -ne 2) {
        throw 'Search data generation produced files other than the two JSON indexes.'
    }

    # Exercise the production resource-inclusion target in a small project. API documentation
    # compilation belongs to the full docs build; this fixture checks HTML and snippet constants.
    [xml]$docsProject = Get-Content (Join-Path $docsRoot 'Blazorise.Docs.csproj') -Raw
    $resourceTarget = $docsProject.SelectSingleNode('/Project/Target[@Name="IncludeGeneratedDocs"]').OuterXml
    $generationTargets = [System.Security.SecurityElement]::Escape((Join-Path $repoRoot 'Build/Blazorise.Docs.Generation.targets'))

    # Restore and build the compiler through its production MSBuild dependency, without dotnet run.
    $pipelineRoot = Join-Path $TestRoot 'BuildPipeline'
    $pipelineOutput = Join-Path $pipelineRoot 'obj/Debug/net11.0/DocsGenerated'
    [System.IO.Directory]::CreateDirectory($pipelineRoot) | Out-Null
    $pipelineProject = Join-Path $pipelineRoot 'BuildPipeline.csproj'
    [System.IO.File]::WriteAllText($pipelineProject, @"
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net11.0</TargetFramework>
    <EnableDefaultCompileItems>false</EnableDefaultCompileItems>
    <GenerateDocsSearchDataOnly>true</GenerateDocsSearchDataOnly>
  </PropertyGroup>
  <Import Project="$generationTargets" />
</Project>
"@)
    Invoke-DotNet build $pipelineProject --configuration Debug '--property:BlazoriseVersion=2.3.3.1' --nologo
    Assert-SearchIndexesMatch $pipelineOutput
    $pipelineLog = @(Invoke-DotNet build $pipelineProject --configuration Debug '--property:BlazoriseVersion=2.3.3.1' --no-restore --nologo)
    $pipelineLog | ForEach-Object { Write-Host $_ }
    foreach ($stage in @('api', 'search')) {
        if (!($pipelineLog -like "*${stage} is up to date.*")) {
            throw "The MSBuild generation pipeline did not reuse the $stage cache."
        }
    }

    $generatedPathXml = [System.Security.SecurityElement]::Escape($generatedRoot)
    $cacheSourceXml = [System.Security.SecurityElement]::Escape((Join-Path $repoRoot 'Documentation/Blazorise.Docs.Compiler/GenerationCache.cs'))
    $sourceFilesXml = [System.Security.SecurityElement]::Escape((Join-Path $repoRoot 'Documentation/Blazorise.Docs.Compiler/DocsSourceFiles.cs'))
    $compilerProjectXml = [System.Security.SecurityElement]::Escape($compilerProject)
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
    <Compile Include="FixturePaths.cs" />
    <Compile Include="$cacheSourceXml" Link="GenerationCache.cs" />
    <Compile Include="$sourceFilesXml" Link="DocsSourceFiles.cs" />
    <ProjectReference Include="$compilerProjectXml" Aliases="DocsCompiler" />
  </ItemGroup>
  <Import Project="$generationTargets" />
  $resourceTarget
</Project>
"@)
    [System.IO.File]::WriteAllText((Join-Path $TestRoot 'FixturePaths.cs'), @'
namespace Blazorise.Docs.Compiler;

internal static class Paths
{
    public static string GeneratedOutputPath { get; set; }
    public static string BlazoriseLibRoot { get; set; }
    public static string BlazoriseExtensionsRoot { get; set; }
    public static string DirPath() => GeneratedOutputPath;
}
'@)
    # Run the checks on .NET 11 independently of PowerShell's own runtime.
    [System.IO.File]::WriteAllText((Join-Path $TestRoot 'Program.cs'), @'
extern alias DocsCompiler;

#region Using directives
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Blazorise.Docs.Compiler;
using Blazorise.Docs.Models;
using CompilerMarkup = DocsCompiler::Blazorise.Docs.Compiler.CodeExamplesMarkup;
using CompilerPaths = DocsCompiler::Blazorise.Docs.Compiler.Paths;
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

var fixtureRoot = Path.Combine( AppContext.BaseDirectory, "cache-fixture" );
var inputRoot = Path.Combine( fixtureRoot, "inputs" );
var outputPath = Path.Combine( fixtureRoot, "output.txt" );
Paths.GeneratedOutputPath = Path.Combine( fixtureRoot, "generated" );
Paths.BlazoriseLibRoot = inputRoot;
Paths.BlazoriseExtensionsRoot = Path.Combine( fixtureRoot, "extensions" );
Directory.CreateDirectory( inputRoot );
Directory.CreateDirectory( Paths.BlazoriseExtensionsRoot );
var inputPath = Path.Combine( inputRoot, "Example.cs" );
File.WriteAllText( inputPath, "first" );
var calls = 0;

bool Generate()
{
    calls++;
    File.WriteAllText( outputPath, "output" );
    return true;
}

void CheckCache( int expectedCalls, bool force = false )
{
    if ( !GenerationCache.Run( "fixture", DocsSourceFiles.Enumerate( inputRoot, "*.cs" ), () => new[] { outputPath }, Generate, force: force )
        || calls != expectedCalls )
    {
        throw new InvalidOperationException( $"Unexpected cache behavior: expected {expectedCalls} generations, got {calls}." );
    }
}

CheckCache( 1 );
var outputTimestamp = File.GetLastWriteTimeUtc( outputPath );
CheckCache( 1 );
if ( File.GetLastWriteTimeUtc( outputPath ) != outputTimestamp )
{
    throw new InvalidOperationException( "A cached output was rewritten." );
}

// Content changes must invalidate the cache even when the timestamp stays the same.
var inputTimestamp = File.GetLastWriteTimeUtc( inputPath );
File.WriteAllText( inputPath, "second" );
File.SetLastWriteTimeUtc( inputPath, inputTimestamp );
CheckCache( 2 );
var addedPath = Path.Combine( inputRoot, "Added.cs" );
File.WriteAllText( addedPath, "added" );
CheckCache( 3 );
File.Delete( addedPath );
CheckCache( 4 );
File.Delete( outputPath );
CheckCache( 5 );
File.WriteAllText( outputPath, "corrupted output" );
CheckCache( 6 );
CheckCache( 7, force: true );
File.WriteAllText( Path.Combine( Paths.GeneratedOutputPath, ".cache", "fixture.json" ), "{" );
CheckCache( 8 );

// A failed generation must not replace the previous successful cache.
var cachePath = Path.Combine( Paths.GeneratedOutputPath, ".cache", "fixture.json" );
var successfulCache = File.ReadAllText( cachePath );
File.WriteAllText( inputPath, "changed before failed generation" );
if ( GenerationCache.Run( "fixture", DocsSourceFiles.Enumerate( inputRoot, "*.cs" ), () => new[] { outputPath }, () => false )
    || File.ReadAllText( cachePath ) != successfulCache )
{
    throw new InvalidOperationException( "A failed generation replaced its cache." );
}
CheckCache( 9 );

// Build artifacts must never become documentation inputs.
foreach ( var directoryName in new[] { "bin", "obj", "__SOURCEGENERATED__" } )
{
    var directory = Path.Combine( inputRoot, directoryName, "net11.0" );
    Directory.CreateDirectory( directory );
    File.WriteAllText( Path.Combine( directory, "Generated.cs" ), "generated" );
}
CheckCache( 9 );
Directory.CreateDirectory( Path.Combine( Paths.BlazoriseExtensionsRoot, "Blazorise.Icons.Noise" ) );
Directory.CreateDirectory( Path.Combine( Paths.BlazoriseExtensionsRoot, "Blazorise.Example" ) );
var apiDirectories = DocsSourceFiles.ApiDirectories().ToArray();
if ( apiDirectories.Length != 2 || apiDirectories.Any( directory => Path.GetFileName( directory ).StartsWith( "Blazorise.Icons" ) ) )
{
    throw new InvalidOperationException( "API inputs include an excluded project." );
}

// Exercise the actual markup renderer in an isolated documentation source tree.
var markupFixtureRoot = Path.Combine( fixtureRoot, "markup" );
var examplesRoot = Path.Combine( markupFixtureRoot, "Documentation", "Blazorise.Docs", "Pages", "Docs", "Examples" );
Directory.CreateDirectory( examplesRoot );
var examplePath = Path.Combine( examplesRoot, "CacheExample.razor" );
File.WriteAllText( examplePath, "<Div>first-marker</Div><link href=\"style.css?v=__BLAZORISE_VERSION__\" />" );
var originalDirectory = Directory.GetCurrentDirectory();
Directory.SetCurrentDirectory( markupFixtureRoot );
CompilerPaths.GeneratedOutputPath = Path.Combine( markupFixtureRoot, "generated" );
var markupPath = Path.Combine( CompilerPaths.GeneratedOutputPath, "Pages", "Docs", "Code", "CacheExampleCode.html" );

void CheckMarkup( string marker, bool force = false )
{
    if ( !new CompilerMarkup().Execute( force ) || !File.ReadAllText( markupPath ).Contains( marker ) )
    {
        throw new InvalidOperationException( $"Markup generation did not produce {marker}." );
    }

    AssertVersion( File.ReadAllText( markupPath ), "2.3.3.1" );
}

try
{
    CheckMarkup( "first-marker" );
    var markupTimestamp = File.GetLastWriteTimeUtc( markupPath );
    CheckMarkup( "first-marker" );
    CheckMarkup( "first-marker", force: true );
    if ( File.GetLastWriteTimeUtc( markupPath ) != markupTimestamp )
    {
        throw new InvalidOperationException( "Unchanged HTML was rewritten." );
    }

    var sourceTimestamp = File.GetLastWriteTimeUtc( examplePath );
    File.WriteAllText( examplePath, File.ReadAllText( examplePath ).Replace( "first-marker", "second-marker" ) );
    File.SetLastWriteTimeUtc( examplePath, sourceTimestamp );
    CheckMarkup( "second-marker" );
    File.WriteAllText( markupPath, "corrupted HTML" );
    CheckMarkup( "second-marker" );
    File.Delete( markupPath );
    CheckMarkup( "second-marker" );
    File.Delete( examplePath );
    if ( !new CompilerMarkup().Execute() || File.ReadAllText( CompilerPaths.ExampleCodeFilesPath() ).Contains( "CacheExampleCode.html" ) )
    {
        throw new InvalidOperationException( "A removed example remains in the resource manifest." );
    }
}
finally
{
    Directory.SetCurrentDirectory( originalDirectory );
}

static void AssertVersion( string text, string version )
{
    if ( !text.Contains( $"?v={version}" ) || text.Contains( "__BLAZORISE_VERSION__" ) )
    {
        throw new InvalidOperationException( $"Expected resolved asset version {version}." );
    }
}
'@)
    Invoke-DotNet build $fixtureProject --configuration Debug '--property:BlazoriseVersion=2.3.3.1' --nologo
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