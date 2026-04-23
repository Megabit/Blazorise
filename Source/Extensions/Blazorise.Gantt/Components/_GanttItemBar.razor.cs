#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.Gantt.Components;

/// <summary>
/// Internal timeline item bar renderer used by the Gantt component.
/// </summary>
public partial class _GanttItemBar : BaseComponent
{
    #region Methods

    /// <inheritdoc />
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( "b-gantt-item" );
        builder.Append( CustomClass );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Specifies the custom CSS class applied to the item bar.
    /// </summary>
    [Parameter] public string CustomClass { get; set; }

    /// <summary>
    /// Notifies when the item bar is clicked.
    /// </summary>
    [Parameter] public EventCallback Clicked { get; set; }

    /// <summary>
    /// Notifies when the mouse button is pressed on the item bar.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> MouseDown { get; set; }

    /// <summary>
    /// Notifies when the mouse button is pressed on the start resize handle.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> StartResizeMouseDown { get; set; }

    /// <summary>
    /// Notifies when the mouse button is pressed on the end resize handle.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> EndResizeMouseDown { get; set; }

    /// <summary>
    /// Determines whether the start resize handle is rendered.
    /// </summary>
    [Parameter] public bool ShowStartResizeHandle { get; set; }

    /// <summary>
    /// Determines whether the end resize handle is rendered.
    /// </summary>
    [Parameter] public bool ShowEndResizeHandle { get; set; }

    /// <summary>
    /// Determines whether the item start continues before the visible timeline range.
    /// </summary>
    [Parameter] public bool ShowStartOverflowIndicator { get; set; }

    /// <summary>
    /// Determines whether the item end continues after the visible timeline range.
    /// </summary>
    [Parameter] public bool ShowEndOverflowIndicator { get; set; }

    /// <summary>
    /// Defines the content rendered inside the item bar.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}