#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Interface for all <see cref="SelectItem{TValue}"/> components.
/// </summary>
public interface ISelectItem
{
    /// <summary>
    /// Checks if the value is equal to the item value.
    /// </summary>
    /// <param name="value">The value to compare.</param>
    /// <returns>A <see cref="bool"/> value indicating whether the value is equal to the item value.</returns>
    bool CompareTo( object value );

    /// <summary>
    /// Gets or sets the item render fragment.
    /// </summary>
    RenderFragment ChildContent { get; set; }
}

/// <summary>
/// Basic type for all <see cref="SelectItem{TValue}"/> components.
/// </summary>
/// <typeparam name="TValue"></typeparam>
public interface ISelectItem<TValue> : ISelectItem
{
    /// <summary>
    /// Gets or sets the item value.
    /// </summary>
    TValue Value { get; set; }
}