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
/// Renders the three-panel report designer layout.
/// </summary>
public partial class _ReportDesignerLayout
{
    /// <summary>
    /// Content shown in the left designer dictionary panel.
    /// </summary>
    [Parameter] public RenderFragment Dictionary { get; set; }

    /// <summary>
    /// Content shown in the central designer surface.
    /// </summary>
    [Parameter] public RenderFragment Surface { get; set; }

    /// <summary>
    /// Content shown in the right designer properties and explorer panel.
    /// </summary>
    [Parameter] public RenderFragment Panel { get; set; }

    /// <summary>
    /// Floating context menu shown above the designer layout.
    /// </summary>
    [Parameter] public RenderFragment ContextMenu { get; set; }
}