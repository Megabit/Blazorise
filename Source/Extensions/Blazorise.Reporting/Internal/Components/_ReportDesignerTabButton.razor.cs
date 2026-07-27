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
    #region Methods

    private Task OnClicked( MouseEventArgs eventArgs )
    {
        if ( Selected is not null )
            return Selected.Invoke( TabKey );

        return Task.CompletedTask;
    }

    private Task OnContextMenu( MouseEventArgs eventArgs )
        => ContextMenu.InvokeAsync( eventArgs );

    #endregion

    #region Properties

    private Color ButtonColor => Active ? ActiveColor : Color.Light;

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
    /// Icon shown before the tab text.
    /// </summary>
    [Parameter] public IconName? Icon { get; set; }

    /// <summary>
    /// Color used by the active tab.
    /// </summary>
    [Parameter] public Color ActiveColor { get; set; } = Color.Primary;

    /// <summary>
    /// Tab button size.
    /// </summary>
    [Parameter] public Size Size { get; set; } = Size.Small;

    /// <summary>
    /// Raised when the tab is selected.
    /// </summary>
    [Parameter] public Func<string, Task> Selected { get; set; }

    /// <summary>
    /// Raised when the tab context menu is requested.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> ContextMenu { get; set; }

    #endregion
}