#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Renders a text editor inside the report designer properties panel.
/// </summary>
public partial class _ReportDesignerTextProperty
{
    /// <summary>
    /// Property label.
    /// </summary>
    [Parameter] public string Label { get; set; }

    /// <summary>
    /// Current text value.
    /// </summary>
    [Parameter] public string Value { get; set; }

    /// <summary>
    /// Prevents editing the value.
    /// </summary>
    [Parameter] public bool ReadOnly { get; set; }

    /// <summary>
    /// Raised when the text value changes.
    /// </summary>
    [Parameter] public EventCallback<string> Changed { get; set; }
}