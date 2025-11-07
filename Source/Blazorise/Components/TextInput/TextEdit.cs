#region Using directives
using System;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Represents a text input control.
/// </summary>
/// <remarks>
/// <para>
/// <b>Obsolete:</b> This component is an alias for <see cref="TextInput"/> and is maintained for backward compatibility only.
/// Please use <see cref="TextInput"/> instead.
/// </para>
/// </remarks>
[Obsolete( "TextEdit has been replaced by TextInput. Please use TextInput instead." )]
public class TextEdit : TextInput
{
    /// <summary>
    /// Gets or sets the current text value of the input.
    /// </summary>
    [Parameter]
    public string Text
    {
        get => Value;
        set => Value = value;
    }

    /// <summary>
    /// Gets or sets the callback that is invoked when the <see cref="Text"/> property changes.
    /// </summary>
    [Parameter]
    public EventCallback<string> TextChanged
    {
        get => ValueChanged;
        set => ValueChanged = value;
    }

    /// <summary>
    /// Gets or sets an expression that identifies the <see cref="Text"/> property.
    /// </summary>
    [Parameter]
    public Expression<Func<string>> TextExpression
    {
        get => ValueExpression;
        set => ValueExpression = value;
    }
}