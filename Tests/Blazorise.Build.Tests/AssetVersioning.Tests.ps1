param(
    [string]$TestRoot = (Join-Path ([System.IO.Path]::GetTempPath()) ("Blazorise.AssetVersioning." + [guid]::NewGuid().ToString('N')))
)

# Run explicitly with PowerShell 7 and the .NET 11 SDK. This builds isolated fixtures,
# not the Blazorise solution, and leaves their outputs available for inspection.
$ErrorActionPreference = 'Stop'
$repoRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$TestRoot = [System.IO.Path]::GetFullPath($TestRoot)
if (Test-Path -LiteralPath $TestRoot) {
    throw "Use a new test directory: $TestRoot"
}

$targetsPath = [System.Security.SecurityElement]::Escape((Join-Path $repoRoot 'Build/Blazorise.Assets.targets'))
$libraryPath = Join-Path $TestRoot 'Library'
$hostPath = Join-Path $TestRoot 'Host'
$packagePath = Join-Path $TestRoot 'packages'
$token = '__BLAZORISE_VERSION__'

function Write-Fixture([string]$Path, [string]$Text) {
    [System.IO.Directory]::CreateDirectory([System.IO.Path]::GetDirectoryName($Path)) | Out-Null
    [System.IO.File]::WriteAllText($Path, $Text)
}

function Invoke-DotNet {
    & dotnet @args
    if ($LASTEXITCODE -ne 0) {
        throw "dotnet failed: $args"
    }
}

function Assert-Equal($Expected, $Actual, [string]$Description) {
    if ($Expected -cne $Actual) {
        throw "$Description. Expected '$Expected', got '$Actual'."
    }
}

function Assert-Assets([string]$Root, [hashtable]$Sources, [string]$Version) {
    foreach ($entry in $Sources.GetEnumerator()) {
        $actual = [System.IO.File]::ReadAllText((Join-Path $Root $entry.Key))
        Assert-Equal $entry.Value.Replace($token, $Version) $actual "Unexpected asset $($entry.Key)"
    }
}

Write-Fixture (Join-Path $libraryPath 'Library.csproj') @"
<Project Sdk="Microsoft.NET.Sdk.Razor">
  <Import Project="$targetsPath" />
  <PropertyGroup>
    <TargetFramework>net11.0</TargetFramework>
    <Version>`$(BlazoriseVersion)</Version>
    <PackageId>AssetFixture</PackageId>
  </PropertyGroup>
  <ItemGroup>
    <FrameworkReference Include="Microsoft.AspNetCore.App" />
  </ItemGroup>
</Project>
"@
Write-Fixture (Join-Path $libraryPath 'Marker.cs') 'namespace AssetFixture; public class Marker { }'
Write-Fixture (Join-Path $hostPath 'Host.csproj') @"
<Project Sdk="Microsoft.NET.Sdk.Web">
  <Import Project="$targetsPath" />
  <PropertyGroup>
    <TargetFramework>net11.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="../Library/Library.csproj" />
  </ItemGroup>
</Project>
"@
Write-Fixture (Join-Path $hostPath 'Program.cs') 'var app = WebApplication.CreateBuilder(args).Build(); app.MapStaticAssets(); app.Run();'

$sources = @{
    'module.js' = 'import "./nested/dependency.mjs?v=__BLAZORISE_VERSION__"; import "./unchanged.js?v=7.0.0";'
    'nested/dependency.mjs' = 'export const stylesheet = "./styles.css?v=__BLAZORISE_VERSION__";'
    'nested/styles.css' = '@import "../base.css?v=__BLAZORISE_VERSION__";'
    'base.css' = 'body { color: black; }'
    'unchanged.js' = 'export const value = 1;'
}
foreach ($entry in $sources.GetEnumerator()) {
    Write-Fixture (Join-Path $libraryPath "wwwroot/$($entry.Key)") $entry.Value
}
$html = '<link rel="stylesheet" href="_content/AssetFixture/nested/styles.css?v=__BLAZORISE_VERSION__" />'
Write-Fixture (Join-Path $hostPath 'wwwroot/index.html') $html

Push-Location $TestRoot
try {
    foreach ($case in @(
        @{ Package = '2.3.3'; Asset = '2.3.3.0' },
        @{ Package = '2.3.3.1'; Asset = '2.3.3.1' }
    )) {
        $property = "-p:BlazoriseVersion=$($case.Package)"
        Invoke-DotNet build (Join-Path $hostPath 'Host.csproj') $property --nologo

        # Inspect primary assets; compressed alternatives share their OriginalItemSpec.
        $manifest = Get-Content (Join-Path $libraryPath 'obj/Debug/net11.0/staticwebassets.build.json') -Raw | ConvertFrom-Json
        foreach ($entry in $sources.GetEnumerator()) {
            $asset = @($manifest.Assets | Where-Object { $_.AssetRole -eq 'Primary' -and $_.OriginalItemSpec.Replace('\', '/').EndsWith('/' + $entry.Key) })
            Assert-Equal 1 $asset.Count "Duplicate or missing manifest entry for $($entry.Key)"
            Assert-Equal $entry.Value.Replace($token, $case.Asset) ([System.IO.File]::ReadAllText($asset[0].Identity)) 'Manifest points at unresolved content'
            $hash = [System.Security.Cryptography.SHA256]::HashData([System.IO.File]::ReadAllBytes($asset[0].Identity))
            Assert-Equal ([Convert]::ToBase64String($hash)) $asset[0].Integrity 'Asset integrity was computed before replacing the version'

            if ($entry.Value.Contains($token)) {
                $expectedRoot = [System.IO.Path]::GetFullPath((Join-Path $libraryPath "obj/Debug/net11.0/blazorise-assets/$($case.Asset)/wwwroot"))
                Assert-Equal $expectedRoot.Replace('\', '/') $asset[0].ContentRoot.Replace('\', '/').TrimEnd('/') 'Development content root points at source templates'
            }
        }

        $generatedModule = Join-Path $libraryPath "obj/Debug/net11.0/blazorise-assets/$($case.Asset)/wwwroot/module.js"
        $timestamp = [System.IO.File]::GetLastWriteTimeUtc($generatedModule)
        Invoke-DotNet build (Join-Path $hostPath 'Host.csproj') $property --no-restore --nologo
        Assert-Equal $timestamp ([System.IO.File]::GetLastWriteTimeUtc($generatedModule)) 'Unchanged output was rewritten'

        Invoke-DotNet pack (Join-Path $libraryPath 'Library.csproj') $property -c Debug --no-build -o $packagePath --nologo
        $archive = [System.IO.Compression.ZipFile]::OpenRead((Join-Path $packagePath "AssetFixture.$($case.Package).nupkg"))
        try {
            foreach ($entry in $sources.GetEnumerator()) {
                $packagedAsset = $archive.GetEntry("staticwebassets/$($entry.Key)")
                if ($null -eq $packagedAsset) {
                    throw "Asset missing from package: $($entry.Key)"
                }
                $reader = [System.IO.StreamReader]::new($packagedAsset.Open())
                try {
                    Assert-Equal $entry.Value.Replace($token, $case.Asset) $reader.ReadToEnd() 'Package contains unresolved content'
                }
                finally {
                    $reader.Dispose()
                }
            }
        }
        finally {
            $archive.Dispose()
        }

        $publishPath = Join-Path $TestRoot "publish-$($case.Package)"
        Invoke-DotNet publish (Join-Path $hostPath 'Host.csproj') $property -c Debug --no-build -o $publishPath --nologo
        Assert-Assets (Join-Path $publishPath 'wwwroot/_content/AssetFixture') $sources $case.Asset
        Assert-Equal $html.Replace($token, $case.Asset) ([System.IO.File]::ReadAllText((Join-Path $publishPath 'wwwroot/index.html'))) 'Published HTML contains an unresolved version'
        Assert-Assets (Join-Path $libraryPath 'wwwroot') $sources $token
        Assert-Equal $html ([System.IO.File]::ReadAllText((Join-Path $hostPath 'wwwroot/index.html'))) 'Source HTML was changed'
    }

    # Removing a token must restore the original asset, even when an older generated copy exists.
    $sources['module.js'] = 'export const value = 2;'
    Write-Fixture (Join-Path $libraryPath 'wwwroot/module.js') $sources['module.js']
    Invoke-DotNet build (Join-Path $hostPath 'Host.csproj') "-p:BlazoriseVersion=2.3.3.1" --no-restore --nologo
    $manifest = Get-Content (Join-Path $libraryPath 'obj/Debug/net11.0/staticwebassets.build.json') -Raw | ConvertFrom-Json
    $asset = @($manifest.Assets | Where-Object { $_.AssetRole -eq 'Primary' -and $_.OriginalItemSpec.Replace('\', '/').EndsWith('/module.js') })
    Assert-Equal 1 $asset.Count 'Removing a token left duplicate assets'
    Assert-Equal $sources['module.js'] ([System.IO.File]::ReadAllText($asset[0].Identity)) 'Removing a token left stale generated content'

    Write-Host "Asset versioning checks passed. Fixtures: $TestRoot"
}
finally {
    Pop-Location
}