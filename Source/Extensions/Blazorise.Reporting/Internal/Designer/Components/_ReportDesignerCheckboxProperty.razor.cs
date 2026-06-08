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
    [Parameter] public EventCallback<ChangeEventArgs> Changed { get; set; }
}