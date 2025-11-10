#region Using directives
using System;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Represents a multi-line text input control.
/// </summary>
/// <remarks>
/// <para>
/// <b>Obsolete:</b> This component is an alias for <see cref="MemoInput"/> and is maintained for backward compatibility only.
/// Please use <see cref="MemoInput"/> instead.
/// </para>
/// <para>
/// This component renders a <c>&lt;textarea&gt;</c> element for editing multi-line text values.
/// The <see cref="Text"/>, <see cref="TextChanged"/>, and <see cref="TextExpression"/> parameters map directly to
/// <see cref="BaseInputComponent{TValue}.Value"/>, <see cref="BaseInputComponent{TValue}.ValueChanged"/>, and <see cref="BaseInputComponent{TValue}.ValueExpression"/> respectively.
/// </para>
/// </remarks>
[Obsolete( "MemoEdit has been replaced by MemoInput. Please use MemoInput instead." )]
public class MemoEdit : MemoInput
{
    /// <summary>
    /// Gets or sets the current text value of the memo input.
    /// </summary>
    [Parameter]
    public string Text
    {
        get => Value;
        set => Value = value;
    }

    /// <summary>
    /// Gets or sets the callback that is invoked when the <see cref="Text"/> value changes.
    /// </summary>
    [Parameter]
    public EventCallback<string> TextChanged
    {
        get => ValueChanged;
        set => ValueChanged = value;
    }

    /// <summary>
    /// Gets or sets an expression that identifies the <see cref="Text"/> field for validation.
    /// </summary>
    [Parameter]
    public Expression<Func<string>> TextExpression
    {
        get => ValueExpression;
        set => ValueExpression = value;
    }
}
