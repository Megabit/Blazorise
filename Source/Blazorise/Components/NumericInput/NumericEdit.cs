#region Using directives
using System;
#endregion

namespace Blazorise;

/// <summary>
/// Represents a numeric input control for editing numeric values.
/// </summary>
/// <remarks>
/// <para>
/// <b>Obsolete:</b> This component is an alias for <see cref="NumericInput{TValue}"/> and is maintained for backward compatibility only.
/// Please use <see cref="NumericInput{TValue}"/> instead.
/// </para>
/// </remarks>
/// <typeparam name="TValue">The type of value to bind, such as <see cref="int"/>, <see cref="decimal"/>, or <see cref="double"/>.</typeparam>
[Obsolete( "NumericEdit has been replaced by NumericInput. Please use NumericInput instead." )]
public class NumericEdit<TValue> : NumericInput<TValue>
{
}