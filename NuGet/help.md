# Publish process

## Documentation

Before creating NuGet packages make sure that documentation is updated to match new version.

1. Update release notes
2. Update _readme.md_
3. Update quick start & usage pages if necessary
4. Make sure that Blazor version number is matching the one in *.props files

## Nugets

### Update project files

1. Open Blazorise.props file in the Build folder and raise the version numbers.
1.1 Update the Version number with the expected assembly version. I.E: 1.5.0
1.1.1 It's important to keep the version number with the format [0.0.0] or [0.0.0.0] as the JS files are using this format to determine the version.
1.2 Update the PackageVersion number with the expected package version. I.E: 1.5.0-preview-1

### Build & Publish

1. Run **dotnet-pack.bat**.2. 
2. Run **push.bat** to publish nuget packages!