## Fody

The naming conventions `Blazorise.Weavers` (bw) and `Blazorise.Weavers.Fody` (bwf) are necessary.
Similarly, the `ModuleWeaver` class in `bwf` follows Fody's required naming convention.

### Purpose of `bw` and `bwf`

- **`Blazorise.Weavers` (bw)**:
    - Exists solely to create a NuGet package (includes `Fody.Packaging`).
    - It acts as a wrapper for `bwf`.

- **`Blazorise.Weavers.Fody` (bwf)**:
    - The actual implementation of the weaving logic.
    - This is the "real hero" performing the weaving tasks.

### Why `bwf` is Not Referenced Directly
The `bwf` project is not referenced directly in any other project because:
- Its purpose is to be put inside nuget package
- Fody in the `Blazorise` project automatically picks up the NuGet package for weaving. The package is created in bw proj.
- Nuget package, that is created from bw (using bwf code) is named `Blazorise.Weavers.Fody`,
exactly the same as the bwf project. And you get a lot of fun with `Ambiguous package name` errors...   

### Challenges and Solutions

Since `bwf` is not referenced:

- It is not built when you build a project like `Blazorise.csproj`.
- To address this, a custom MSBuild target was added to `bw` to explicitly run `dotnet build` on `bwf`.
   - And can be turned off with `-p:BuildBlazoriseWeaverDll=false`.
- bwf dll is there

### Current Workflow

- The DLL from `bwf` is "hardcoded" as a reference in `Blazorise` (via `WeaverFiles`). (it's taken from official docs)
- This ensures that the weaver logic is available during the build (exactly where it should be).

### Future Considerations

Creating a NuGet package for both `bwf` and `bw` would simplify integration.
However - harder to make changes to the weaver (build package, change package, etc...).

https://github.com/Fody/Home/blob/master/pages/addin-development.md