#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.Gantt.Components;

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

    [Parameter] public string CustomClass { get; set; }

    [Parameter] public EventCallback Clicked { get; set; }

    [Parameter] public EventCallback<MouseEventArgs> MouseDown { get; set; }

    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}