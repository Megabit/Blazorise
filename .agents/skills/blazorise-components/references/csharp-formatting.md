# C# formatting examples

Use these examples alongside the coding rules in the repository's [AGENTS.md](../../../../AGENTS.md). Readability determines wrapping; there is no fixed line-length limit.

#### Short Boolean declarations

Good: keep a short lookup and comparison together.

```csharp
var valueChanged = parameters.TryGetValue<int>( nameof( Value ), out var paramValue ) && paramValue != Value;
```

Bad: wrapping a short declaration solely at the Boolean operator.

```csharp
var valueChanged = parameters.TryGetValue<int>( nameof( Value ), out var paramValue )
    && paramValue != Value;
```

#### Multiline Boolean declarations

Good: separate distinct method-call checks when that makes their purpose and arguments easier to scan. This also applies to local declarations, not only to `if` and `return` expressions.

```csharp
var selectedPanelChanged = !string.IsNullOrWhiteSpace( selectedPanelTab )
    && !string.Equals( selectedPanelTab, SelectedPanelTab, StringComparison.Ordinal );
```

Bad: flattening the two checks solely because there are only two operands.

```csharp
var selectedPanelChanged = !string.IsNullOrWhiteSpace( selectedPanelTab ) && !string.Equals( selectedPanelTab, SelectedPanelTab, StringComparison.Ordinal );
```

The short lookup-and-comparison example above is one tightly related operation. A presence check followed by a separate comparison or validation call can benefit from multiline formatting. Neither operand count nor line length alone decides the layout.

#### Multiline conditions

Good: keep distinct checks on separate lines when that makes an `if` condition easier to scan. Both of these forms are allowed, even if they could fit on one line.

```csharp
if ( parts.Length == 2
    && int.TryParse( parts[0], out var width )
    && int.TryParse( parts[1], out var height ) )
{
    return (double)width / height;
}

if ( string.Equals( host, "localhost", StringComparison.OrdinalIgnoreCase )
    || host.EndsWith( ".localhost", StringComparison.OrdinalIgnoreCase ) )
{
    throw new InvalidOperationException( "Localhost is not allowed." );
}
```

Bad: placing continuation operators at the end of the preceding line.

```csharp
if ( parts.Length == 2 &&
    int.TryParse( parts[0], out var width ) &&
    int.TryParse( parts[1], out var height ) )
{
    return (double)width / height;
}
```

Do not mechanically flatten existing multiline conditions to match the short-declaration example. Single-line `if` conditions are also allowed when they read clearly.

#### Boolean return expressions

Good: keep a series of related comparisons on separate lines, with continuation operators indented one level (four spaces) from the `return` statement, not aligned under its first operand.

```csharp
return left < otherLeft + otherWidth
    && left + width > otherLeft
    && top < otherTop + otherHeight
    && top + height > otherTop;
```

Bad: flattening the comparison group into a single line merely because it fits.

```csharp
return left < otherLeft + otherWidth && left + width > otherLeft && top < otherTop + otherHeight && top + height > otherTop;
```

Short, direct return expressions may still stay on one line. Preserve multiline grouping when it makes the individual checks easier to compare.

#### Expression-bodied Boolean members

Good: keep distinct checks on separate lines and indent `=>` one level from the declaration.

```csharp
private static bool IsPaneSizeValue( string size )
    => size.IndexOf( "fr", StringComparison.Ordinal ) < 0
        && !size.StartsWith( "minmax(", StringComparison.OrdinalIgnoreCase );
```

Bad: flattening the checks just because the member uses an expression body.

```csharp
private static bool IsPaneSizeValue( string size )
    => size.IndexOf( "fr", StringComparison.Ordinal ) < 0 && !size.StartsWith( "minmax(", StringComparison.OrdinalIgnoreCase );
```

For this multiline Boolean property layout, align continuation operators under the first operand after `=>` (three spaces beyond the arrow's indentation).

Good:

```csharp
private bool IsTabbableWithoutHref
    => ParentDropdownState?.Mode == BarMode.Horizontal
       && !Disabled
       && string.IsNullOrEmpty( To );
```

Bad: shifting the continuation operators one space farther right.

```csharp
private bool IsTabbableWithoutHref
    => ParentDropdownState?.Mode == BarMode.Horizontal
        && !Disabled
        && string.IsNullOrEmpty( To );
```

Expression bodies do not require single-line expressions. Apply the same readability rules as for Boolean declarations and return statements.

#### Spacing around multiline declarations

Good: keep related single-line declarations together and separate each multiline declaration with a blank line.

```csharp
var wasRendered = Rendered;
var shouldEvaluate = !Rendered || parameters.IsParameterChanged( Value );

var rulesChanged = parameters.IsParameterChanged( MinimumLength )
    || parameters.IsParameterChanged( RequireUppercase )
    || parameters.IsParameterChanged( RequireLowercase );

var shouldUpdateBlockedPasswords = !Rendered
    || parameters.IsParameterChanged( UseDefaultBlockedPasswordList )
    || parameters.TryGetValue<IEnumerable<string>>( nameof( BlockedPasswords ), out _ );

await base.SetParametersAsync( parameters );
```

Bad: running multiline declarations into adjacent statements.

```csharp
var wasRendered = Rendered;
var shouldEvaluate = !Rendered || parameters.IsParameterChanged( Value );
var rulesChanged = parameters.IsParameterChanged( MinimumLength )
    || parameters.IsParameterChanged( RequireUppercase )
    || parameters.IsParameterChanged( RequireLowercase );
var shouldUpdateBlockedPasswords = !Rendered
    || parameters.IsParameterChanged( UseDefaultBlockedPasswordList )
    || parameters.TryGetValue<IEnumerable<string>>( nameof( BlockedPasswords ), out _ );
await base.SetParametersAsync( parameters );
```