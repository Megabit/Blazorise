# Publish process

## Documentation

Before creating NuGet packages make sure that documentation is updated to match new version.

1. Update release notes
2. Update _readme.md_
3. Update quick start & usage pages if necessary
4. Update version number in the demo home-page
5. Make sure that Blazor version number is matching the one in *.props files

## Nugets

### Update project files

1. Open Blazorise.props file in the Build folder and raise the version numbers.

### Build & Publish

1. Run **dotnet-pack.bat**.2. 
2. Run **push.bat** to publish nuget packages!