#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Container for the visible context menu commands.
/// </summary>
public partial class ContextMenuBody : BaseComponent
{
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
        builder.Append( $"left:{ParentContextMenu?.ClientX ?? 0}px" );
        builder.Append( $"top:{ParentContextMenu?.ClientY ?? 0}px" );

        if ( MinWidth is not null )
            builder.Append( $"min-width:{MinWidth}" );

        base.BuildStyles( builder );
    }

    #endregion

    #region Properties

    private bool IsVisible => Visible ?? ParentContextMenu?.Visible == true;

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="ContextMenuBody"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Overrides the parent menu visibility.
    /// </summary>
    [Parameter] public bool? Visible { get; set; }

    /// <summary>
    /// Sets a minimum menu width.
    /// </summary>
    [Parameter] public string MinWidth { get; set; }

    /// <summary>
    /// Provides the reference to the parent <see cref="ContextMenu"/> component.
    /// </summary>
    [CascadingParameter] protected ContextMenu ParentContextMenu { get; set; }

    #endregion
}