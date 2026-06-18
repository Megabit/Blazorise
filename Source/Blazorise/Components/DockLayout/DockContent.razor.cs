#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// The central content region of a <see cref="DockLayout"/>.
/// </summary>
public partial class DockContent : BaseComponent
{
    #region Members

    private DockNodeState node;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.DockContent() );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        ParentDockLayout?.RegisterContent( this );
        ParentCollector?.AddNode( Node );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="DockContent"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    [CascadingParameter] internal DockLayout ParentDockLayout { get; set; }

    [CascadingParameter] internal DockNodeCollector ParentCollector { get; set; }

    internal DockNodeState Node => node ??= new()
    {
        Kind = DockNodeKind.Content,
    };

    #endregion
}