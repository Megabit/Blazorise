# ApiDocsGenerator

## Which Component Does It Pick Up?

Either one of these cases:

- The component is a descendant of `Blazorise.BaseComponent`.
- The component is a descendant of `ComponentBase`.
- The component implements `IComponent`.

### Problem with Razor Files

- It doesn't pick up the parameters from Razor files.
- For inheritance to be "read," it needs to be explicitly specified inside the `.cs` file:
  - This is especially true for `ComponentBase`, which is inherited by default when creating a `.razor` file.
    - You need to explicitly declare `SomeComponent : ComponentBase`.

This limitation can be overcome by scanning through the generated files (`SomeComponent.razor.cs`).

## System.Runtime

Retrieving XML comments from the `System.Runtime` assembly can be problematic, particularly for the `IDisposable` interface.

When accessing `AllInterfaces` for a class, the `IDisposable` interface is often represented as an `ErrorTypeSymbol`. This means it is not treated as a "normal" reference, and its members, such as the `Dispose` method, are not recognized. As a result, XML comments for `IDisposable` cannot be resolved.

This issue is likely caused by the implicit nature of the reference to `System.Runtime.dll` in modern .NET projects, where it is part of the shared framework and not explicitly included as a standalone reference.


> Under construction

Things I don't want to miss documenting:

- how to run/test the source generator (sg)
  - dotnet bulild vs launchSettings
  - file watch to run dotnet build
  - The emitted output (inside the proj that reference the sg under Dependencies -> .NET -> Source Generators)
  - On exceptions, it removes the files, no logs. All dies. (haven't explored fully yet)
- PrivateAssets="all" (will not be part of nuget package)
- Why logging is difficult, custom logging
- What it can do..



how to debug it

```json
{
  "$schema": "http://json.schemastore.org/launchsettings.json",
  "profiles": {
    "Generators": {
      "commandName": "DebugRoslynComponent",
      "targetProject": "../../Blazorise/Blazorise.csproj"
    }
  }
}
```

## Default Values

The source generator (SG) is designed to pick up almost any default value.

```csharp
[Parameter] public string SomeValue { get; set; } = "some string value";
```

It also handles cases where the default value is not a constant.

```csharp
[Parameter] public TimeSpan Interval { get; set; } = TimeSpan.FromSeconds(10);
```

Additionally, it works when the default value is set by a backing field.

```csharp
private string _backingField = "backed value";
[Parameter] public string BackedValue 
{
    get => _backingField;
    set => _backingField = value;
}
```

Moreover, it preserves the exact expression of default values, not just the computed result.

```csharp
[Parameter] public int ComplexValue { get; set; } = 20 * 200; // Output: "20 * 200", not "4000"
```

## Where does SG see?

If I add the sg project in docs project (that referenes Blazorise). It can pickup the types, but cannot "read"
the xml comment. 

> SyntaxTree is not part of the compilation (Parameter 'syntaxTree')

https://stackoverflow.com/a/69307775/1154773

https://github.com/dotnet/roslyn/discussions/50874