#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Defines inline content that opens the parent <see cref="ContextMenu"/> when right-clicked.
/// </summary>
public partial class ContextMenuToggle : BaseComponent
{
    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        ParentContextMenu?.NotifyToggleInitialized( this );
    }

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        if ( disposing )
            ParentContextMenu?.NotifyToggleRemoved( this );

        base.Dispose( disposing );
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.ContextMenuToggle() );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="ContextMenuToggle"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Provides the reference to the parent <see cref="ContextMenu"/> component.
    /// </summary>
    [CascadingParameter] protected ContextMenu ParentContextMenu { get; set; }

    #endregion
}