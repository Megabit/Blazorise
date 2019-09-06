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

### Update nuspec files

1. Go through all *.nuspec files and raise version number to be the same as in the Blazorise.props
2. Make sure the Blazor version is matching the version in the .props file.

### Build & Publish

1. First make sure that **build.cmd** is pointing to the valid Visual Studio installation folder.
2. Open command prompt in the NuGet folder and run the **pack** command. Wait for the build to finish!
2. Run **push** command to publish nuget packages!