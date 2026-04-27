#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Gantt.Components;

/// <summary>
/// Internal body container used by the Gantt component.
/// </summary>
public partial class _GanttBody : BaseComponent
{
    #region Methods

    /// <inheritdoc />
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( "b-gantt-body" );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Defines the content rendered inside the body container.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}