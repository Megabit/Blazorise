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
    /// Gets or sets the custom CSS class applied to the item bar.
    /// </summary>
    [Parameter] public string CustomClass { get; set; }

    /// <summary>
    /// Gets or sets callback invoked when the item bar is clicked.
    /// </summary>
    [Parameter] public EventCallback Clicked { get; set; }

    /// <summary>
    /// Gets or sets callback invoked when the mouse button is pressed on the item bar.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> MouseDown { get; set; }

    /// <summary>
    /// Gets or sets callback invoked when the mouse button is pressed on the start resize handle.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> StartResizeMouseDown { get; set; }

    /// <summary>
    /// Gets or sets callback invoked when the mouse button is pressed on the end resize handle.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> EndResizeMouseDown { get; set; }

    /// <summary>
    /// Gets or sets whether the start resize handle is rendered.
    /// </summary>
    [Parameter] public bool ShowStartResizeHandle { get; set; }

    /// <summary>
    /// Gets or sets whether the end resize handle is rendered.
    /// </summary>
    [Parameter] public bool ShowEndResizeHandle { get; set; }

    /// <summary>
    /// Gets or sets the content rendered inside the item bar.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}