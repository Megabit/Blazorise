#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Renders a nullable numeric editor inside the report designer properties panel.
/// </summary>
public partial class _ReportDesignerNullableNumberProperty
{
    /// <summary>
    /// Property label.
    /// </summary>
    [Parameter] public string Label { get; set; }

    /// <summary>
    /// Current nullable numeric value.
    /// </summary>
    [Parameter] public double? Value { get; set; }

    /// <summary>
    /// Raised when the nullable numeric value changes.
    /// </summary>
    [Parameter] public EventCallback<double?> Changed { get; set; }

    /// <summary>
    /// Increment used by the numeric editor.
    /// </summary>
    [Parameter] public decimal? Step { get; set; } = 1m;
}