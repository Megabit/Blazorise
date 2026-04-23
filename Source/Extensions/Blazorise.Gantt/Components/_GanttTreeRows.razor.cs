#region Using directives
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.Gantt.Components;

/// <summary>
/// Internal tree rows host used by the Gantt component.
/// </summary>
public partial class _GanttTreeRows : BaseComponent
{
    #region Members

    private Div treeRowsRef;

    #endregion

    #region Methods

    /// <summary>
    /// Moves keyboard focus to the tree rows host.
    /// </summary>
    /// <returns>A task that completes when the focus request has been issued.</returns>
    public Task FocusAsync()
    {
        if ( treeRowsRef is null )
            return Task.CompletedTask;

        return treeRowsRef.ElementRef.FocusAsync().AsTask();
    }

    /// <inheritdoc />
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( "b-gantt-tree-rows" );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Defines the content rendered inside the tree rows host.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Specifies the tab index used for keyboard focus.
    /// </summary>
    [Parameter] public int TabIndex { get; set; }

    /// <summary>
    /// Defines keyboard event callback.
    /// </summary>
    [Parameter] public EventCallback<KeyboardEventArgs> KeyDown { get; set; }

    #endregion
}