#region Using directives
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.Gantt.Components;

public partial class _GanttTreeRows : BaseComponent
{
    #region Members

    private Div treeRowsRef;

    #endregion

    #region Methods

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

    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the tab index used for keyboard focus.
    /// </summary>
    [Parameter] public int TabIndex { get; set; }

    /// <summary>
    /// Gets or sets keyboard event callback.
    /// </summary>
    [Parameter] public EventCallback<KeyboardEventArgs> KeyDown { get; set; }

    #endregion
}