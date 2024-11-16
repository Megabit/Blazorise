# ApiDocsGenerator

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

