#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Renders a numeric property editor.
/// </summary>
/// <typeparam name="TValue">The numeric value type.</typeparam>
public partial class PropertyGridNumericItem<TValue> : BasePropertyGridEditorItem
{
    private static TValue ConvertValue( double value )
        => Converters.ChangeType<TValue>( value );

    /// <summary>
    /// Gets or sets the property value.
    /// </summary>
    [Parameter] public TValue Value { get; set; }

    /// <summary>
    /// Occurs when the property value changes.
    /// </summary>
    [Parameter] public EventCallback<TValue> ValueChanged { get; set; }

    /// <summary>
    /// Defines the minimum accepted value.
    /// </summary>
    [Parameter] public double Min { get; set; }

    /// <summary>
    /// Defines the maximum accepted value.
    /// </summary>
    [Parameter] public double? Max { get; set; }

    /// <summary>
    /// Defines the increment used by the numeric editor.
    /// </summary>
    [Parameter] public decimal? Step { get; set; } = 1m;

    /// <summary>
    /// Defines whether value changes are reported immediately.
    /// </summary>
    [Parameter] public bool Immediate { get; set; } = true;
}
