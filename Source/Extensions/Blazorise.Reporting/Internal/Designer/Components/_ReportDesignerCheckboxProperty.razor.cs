#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Renders a checkbox editor inside the report designer properties panel.
/// </summary>
public partial class _ReportDesignerCheckboxProperty
{
    /// <summary>
    /// Property label.
    /// </summary>
    [Parameter] public string Label { get; set; }

    /// <summary>
    /// Current checkbox value.
    /// </summary>
    [Parameter] public bool Value { get; set; }

    /// <summary>
    /// Raised when the checkbox value changes.
    /// </summary>
    [Parameter] public EventCallback<bool> Changed { get; set; }
}