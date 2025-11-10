#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Represents a basic text input control.
/// </summary>
/// <remarks>
/// This component is functionally identical to <see cref="TextInput"/> and serves as a semantic alias
/// for compatibility or stylistic preference.
/// <para>
/// It renders a standard <c>&lt;input type="text"&gt;</c> element and supports all the same parameters,
/// including <c>Value</c>, <c>ValueChanged</c>, and <c>ValueExpression</c>.
/// </para>
/// </remarks>
public class Input : TextInput
{
}