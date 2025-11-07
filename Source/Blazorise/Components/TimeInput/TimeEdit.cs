#region Using directives
using System;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Represents a time input control.
/// </summary>
/// <remarks>
/// <para>
/// <b>Obsolete:</b> This component is an alias for <see cref="TimeInput{TValue}"/> and is maintained for backward compatibility only.
/// Please use <see cref="TimeInput{TValue}"/> instead.
/// </para>
/// <para>
/// The properties <see cref="Time"/>, <see cref="TimeChanged"/>, and <see cref="TimeExpression"/> map to
/// <see cref="BaseInputComponent{TValue}.Value"/>, <see cref="BaseInputComponent{TValue}.ValueChanged"/>, and
/// <see cref="BaseInputComponent{TValue}.ValueExpression"/> respectively.
/// </para>
/// </remarks>
/// <typeparam name="TValue">
/// The bound value type (e.g., <see cref="TimeSpan"/> or <see cref="DateTime"/> when representing a time value).
/// </typeparam>
[Obsolete( "TimeEdit has been replaced by TimeInput. Please use TimeInput instead." )]
public class TimeEdit<TValue> : TimeInput<TValue>
{
    /// <summary>
    /// Gets or sets the current time value.
    /// </summary>
    [Parameter]
    public TValue Time
    {
        get => Value;
        set => Value = value;
    }

    /// <summary>
    /// Gets or sets the callback that is invoked when <see cref="Time"/> changes.
    /// </summary>
    [Parameter]
    public EventCallback<TValue> TimeChanged
    {
        get => ValueChanged;
        set => ValueChanged = value;
    }

    /// <summary>
    /// Gets or sets an expression that identifies the <see cref="Time"/> field for validation.
    /// </summary>
    [Parameter]
    public Expression<Func<TValue>> TimeExpression
    {
        get => ValueExpression;
        set => ValueExpression = value;
    }
}