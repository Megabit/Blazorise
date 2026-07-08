#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Renders a report designer document tab.
/// </summary>
public partial class _ReportDesignerTabButton
{
    private Color ButtonColor => Active ? Color.Primary : Color.Light;

    private Task OnClicked( MouseEventArgs eventArgs )
    {
        if ( Selected is not null )
            return Selected.Invoke( TabKey );

        return Task.CompletedTask;
    }

    /// <summary>
    /// Stable tab key.
    /// </summary>
    [Parameter] public string TabKey { get; set; }

    /// <summary>
    /// Tab display text.
    /// </summary>
    [Parameter] public string Text { get; set; }

    /// <summary>
    /// Indicates whether the tab is active.
    /// </summary>
    [Parameter] public bool Active { get; set; }

    /// <summary>
    /// Raised when the tab is selected.
    /// </summary>
    [Parameter] public Func<string, Task> Selected { get; set; }
}