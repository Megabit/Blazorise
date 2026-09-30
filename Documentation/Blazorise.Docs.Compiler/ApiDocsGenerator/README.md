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

The generator reads property initializers and simple getters backed by initialized fields. It does not evaluate constructors, lifecycle methods, nested state objects, theme settings, or other runtime logic.

When the effective default cannot be inferred from the property declaration, add a `Defaults to <c>value</c>.` sentence to the property's `<summary>`:

```csharp
/// <summary>
/// Specifies the maximum number of items per page. Defaults to <c>10</c>.
/// </summary>
[Parameter] public int PageSize { get => paginationContext.PageSize; set => paginationContext.PageSize = value; }
```

The documented value overrides the Default column and the API search index without changing runtime behavior. The generator omits the sentence from the API description, while XML documentation and IntelliSense retain it. Keep the default sentence on the same comment line when the existing summary text fits on one line. Use a simple value, and explain inheritance or conditions in `<remarks>`. If there is no single default, use a short label such as `Provider default`:

```csharp
/// <summary>
/// Specifies whether the dropdown toggle icon is visible. Defaults to <c>true</c>.
/// </summary>
/// <remarks>
/// When unspecified, uses the theme setting, falling back to true.
/// </remarks>
[Parameter] public bool? ShowToggleIcon { get; set; }
```

Use the exact sentence form `Defaults to <c>value</c>.`, either directly in `<summary>` or in a summary's `<para>`. The inline code element identifies the complete value, including decimal points, enum names, quoted strings, or comma-separated lists. Use XML entities such as `&lt;` and `&amp;` for special characters. Whitespace is collapsed to a single line.

Only this sentence form overrides inference: plain prose, code examples, remarks, empty values, and conditional phrases such as `Defaults to <c>10</c> when enabled.` do not. For properties using `<inheritdoc/>`, a local documented default takes precedence over defaults inherited from an overridden property or an implemented interface.

Do not add property initializers solely to fix documentation: they can bypass theme, parent, or global-option fallbacks. Existing `<remarks>Default: ...</remarks>` comments remain explanatory text and do not override the Default column.

For ordinary property initializers, no default sentence is needed:

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