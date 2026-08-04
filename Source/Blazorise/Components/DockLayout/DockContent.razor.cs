#region Using directives
using System;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// The central content region of a <see cref="DockLayout"/>.
/// </summary>
public partial class DockContent : BaseComponent, IDisposable
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
    protected override void OnInitialized()
    {
        base.OnInitialized();

        ParentCollector?.AddNode( Node );

        if ( ParentDockLayout?.RegisterContent( this ) == true )
            ExecuteAfterRender( ParentDockLayout.NotifyDefinitionChanged );
    }

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        if ( disposing )
        {
            ParentCollector?.RemoveNode( Node );
            ParentDockLayout?.UnregisterContent( this );
        }

        base.Dispose( disposing );
    }

    #endregion

    #region Properties

    internal DockNodeState Node => node ??= new()
    {
        Kind = DockNodeKind.Content,
    };

    [CascadingParameter] internal DockLayout ParentDockLayout { get; set; }

    [CascadingParameter] internal DockNodeCollector ParentCollector { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="DockContent"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}