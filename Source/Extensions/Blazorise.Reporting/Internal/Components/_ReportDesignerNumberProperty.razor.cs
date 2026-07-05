#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Renders a numeric editor inside the report designer properties panel.
/// </summary>
/// <typeparam name="TValue">Numeric value type edited by the property.</typeparam>
public partial class _ReportDesignerNumberProperty<TValue>
{
    /// <summary>
    /// Property label.
    /// </summary>
    [Parameter] public string Label { get; set; }

    /// <summary>
    /// Current numeric value.
    /// </summary>
    [Parameter] public TValue Value { get; set; }

    /// <summary>
    /// Raised when the numeric value changes.
    /// </summary>
    [Parameter] public EventCallback<TValue> Changed { get; set; }

    /// <summary>
    /// Increment used by the numeric editor.
    /// </summary>
    [Parameter] public decimal? Step { get; set; } = 1m;
}
