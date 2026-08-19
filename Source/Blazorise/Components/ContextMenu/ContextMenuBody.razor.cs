#region Using directives
using Blazorise.States;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Container for the visible context menu commands.
/// </summary>
public partial class ContextMenuBody : BaseComponent
{
    #region Members

    private ContextMenuState parentContextMenuState;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        ParentContextMenu?.NotifyBodyInitialized( this );

        base.OnInitialized();
    }

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        if ( disposing )
            ParentContextMenu?.NotifyBodyRemoved( this );

        base.Dispose( disposing );
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.ContextMenuBody() );
        builder.Append( ClassProvider.ContextMenuBodyVisible( true ) );
        builder.Append( ClassProvider.ContextMenuBodyPositionStrategy( DropdownPositionStrategy.Fixed ) );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override void BuildStyles( StyleBuilder builder )
    {
        base.BuildStyles( builder );

        builder.Append( "visibility:hidden", ParentContextMenu?.IsFloatingPositionInitialized != true );
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    private bool IsVisible => ParentContextMenuState?.Visible == true;

    /// <summary>
    /// Gets the state of the parent <see cref="ContextMenu"/> component.
    /// </summary>
    [CascadingParameter]
    protected ContextMenuState ParentContextMenuState
    {
        get => parentContextMenuState;
        set
        {
            if ( parentContextMenuState == value )
                return;

            parentContextMenuState = value;

            if ( value?.Visible != true )
                DirtyStyles();
        }
    }

    /// <summary>
    /// Provides the reference to the parent <see cref="ContextMenu"/> component.
    /// </summary>
    [CascadingParameter] protected ContextMenu ParentContextMenu { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="ContextMenuBody"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}