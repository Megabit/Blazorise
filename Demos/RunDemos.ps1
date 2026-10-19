param(
    [Parameter(Position = 0)]
    [ValidateSet('Debug', 'DebugNet11', 'Release')]
    [string] $Configuration = 'DebugNet11',
    [switch] $Watch,
    [switch] $NoBuild,
    [ValidateRange(1, 3600)]
    [int] $StartupTimeoutSeconds = 300
)

$ErrorActionPreference = 'Stop'
$demoSolutionPath = Join-Path $PSScriptRoot 'Blazorise.Demos.slnx'

try {
    if ($Watch -and $NoBuild) {
        throw '-NoBuild is only supported by DotnetRunAll.bat. Watch manages its own builds.'
    }

    $dotnetPath = (Get-Command dotnet -CommandType Application -ErrorAction Stop).Source

    # Use the same project list for building and launching, excluding MAUI and Server.
    $demoSolution = [xml] (Get-Content -Raw -LiteralPath $demoSolutionPath)
    $demos = foreach ($demoProject in $demoSolution.Solution.Project) {
        $project = Join-Path (Split-Path -Parent $demoSolutionPath) $demoProject.Path
        $directory = Split-Path -Parent $project
        $name = [System.IO.Path]::GetFileNameWithoutExtension($project)

        if (-not (Test-Path -LiteralPath $project -PathType Leaf)) {
            throw "Demo project not found: $project"
        }

        $settings = Get-Content -Raw -LiteralPath (Join-Path $directory 'Properties\launchSettings.json') | ConvertFrom-Json
        $profile = $settings.profiles.PSObject.Properties |
            Where-Object { $_.Value.commandName -eq 'Project' } |
            Select-Object -First 1

        if (-not $profile -or -not $profile.Value.applicationUrl) {
            throw "No Project launch profile with an applicationUrl was found for $name."
        }

        $applicationUrl = ($profile.Value.applicationUrl -split ';')[0].TrimEnd('/')
        $launchUrl = [string] $profile.Value.launchUrl
        $url = "$applicationUrl/$($launchUrl.TrimStart('/'))"

        [pscustomobject]@{
            Name = $name
            Directory = $directory
            Project = $project
            Profile = $profile.Name
            CommandLineArgs = [string] $profile.Value.commandLineArgs
            Url = $url
        }
    }

    if (-not $Watch -and -not $NoBuild) {
        # One MSBuild invocation shares dependency results across demos and schedules independent work in parallel.
        # Preserve the chosen configuration for shared projects referenced from outside the demos-only solution.
        Write-Host "Building all demos ($Configuration)..."
        & $dotnetPath build $demoSolutionPath --configuration $Configuration -maxcpucount -p:ShouldUnsetParentConfigurationAndPlatform=false

        if ($LASTEXITCODE -ne 0) {
            throw 'Demo build failed. No demos have been started.'
        }
    }

    # Watch startup is also sequential: wait for each initial build and host before starting the next.
    $previousSuppressLaunchBrowser = $env:DOTNET_WATCH_SUPPRESS_LAUNCH_BROWSER
    $env:DOTNET_WATCH_SUPPRESS_LAUNCH_BROWSER = '1'

    try {
        foreach ($demo in $demos) {
            $arguments = if ($Watch) {
                @('watch', 'run')
            }
            else {
                @('run', '--no-build', '--no-restore')
            }

            # The working directory identifies the project; the launch profile supplies ports and environment variables.
            $arguments += @('--configuration', $Configuration, '--launch-profile', ('"{0}"' -f $demo.Profile))

            # WebAssembly defines RunArguments, so dotnet run ignores the profile's commandLineArgs unless forwarded explicitly.
            if (-not [string]::IsNullOrWhiteSpace($demo.CommandLineArgs)) {
                $arguments += @('--', $demo.CommandLineArgs)
            }

            Write-Host "Starting $($demo.Name): $($demo.Url)"
            $process = Start-Process -FilePath $dotnetPath -ArgumentList $arguments -WorkingDirectory $demo.Directory -PassThru
            $timer = [System.Diagnostics.Stopwatch]::StartNew()
            $ready = $false

            while ($timer.Elapsed.TotalSeconds -lt $StartupTimeoutSeconds) {
                if ($process.HasExited) {
                    throw "$($demo.Name) exited during startup with code $($process.ExitCode)."
                }

                try {
                    $response = Invoke-WebRequest -Uri $demo.Url -UseBasicParsing -TimeoutSec 2
                    $ready = $response.StatusCode -eq 200

                    # The SPA fallback can return index.html even when the path base is wrong and assets return 404.
                    foreach ($assetPath in @('_framework/blazor.webassembly.js', '_content/Blazorise.Demo/demo.css')) {
                        if (-not $ready) {
                            break
                        }

                        $assetUrl = [uri]::new([uri] $demo.Url, $assetPath)
                        $assetResponse = Invoke-WebRequest -Uri $assetUrl -UseBasicParsing -TimeoutSec 2
                        $ready = $assetResponse.StatusCode -eq 200 -and [string] $assetResponse.Headers['Content-Type'] -match '(javascript|css)'
                    }
                }
                catch {
                    $ready = $false
                }

                if ($ready) {
                    break
                }

                Start-Sleep -Milliseconds 500
            }

            if (-not $ready) {
                throw "$($demo.Name) did not serve its page and static assets within $StartupTimeoutSeconds seconds. Check its console window."
            }

            if (-not $Watch) {
                Start-Process -FilePath $demo.Url
            }
        }
    }
    finally {
        $env:DOTNET_WATCH_SUPPRESS_LAUNCH_BROWSER = $previousSuppressLaunchBrowser
    }

    Write-Host "All $($demos.Count) demos are running. Press Ctrl+C in each demo console to stop it."
    exit 0
}
catch {
    Write-Host $_.Exception.Message -ForegroundColor Red
    Write-Host 'Any demos already started remain running in their own console windows.'
    exit 1
}