#region Using directives
using System;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Represents a date input control.
/// </summary>
/// <remarks>
/// <para>
/// <b>Obsolete:</b> This component is an alias for <see cref="DateInput{TValue}"/> and is maintained for backward compatibility only.
/// Please use <see cref="DateInput{TValue}"/> instead.
/// </para>
/// <para>
/// The properties <see cref="Date"/>, <see cref="DateChanged"/>, and <see cref="DateExpression"/> map to
/// <see cref="BaseInputComponent{TValue}.Value"/>, <see cref="BaseInputComponent{TValue}.ValueChanged"/>, and
/// <see cref="BaseInputComponent{TValue}.ValueExpression"/> respectively.
/// </para>
/// </remarks>
/// <typeparam name="TValue">
/// The bound value type (e.g., <see cref="DateTime"/> or <see cref="DateOnly"/>).
/// </typeparam>
[Obsolete( "DateEdit has been replaced by DateInput. Please use DateInput instead." )]
public class DateEdit<TValue> : DateInput<TValue>
{
    /// <summary>
    /// Gets or sets the current date value.
    /// </summary>
    [Parameter]
    public TValue Date
    {
        get => Value;
        set => Value = value;
    }

    /// <summary>
    /// Gets or sets the callback that is invoked when <see cref="Date"/> changes.
    /// </summary>
    [Parameter]
    public EventCallback<TValue> DateChanged
    {
        get => ValueChanged;
        set => ValueChanged = value;
    }

    /// <summary>
    /// Gets or sets an expression that identifies the <see cref="Date"/> field for validation.
    /// </summary>
    [Parameter]
    public Expression<Func<TValue>> DateExpression
    {
        get => ValueExpression;
        set => ValueExpression = value;
    }
}