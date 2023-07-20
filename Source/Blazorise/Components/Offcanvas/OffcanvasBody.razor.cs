#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Center area of the offcanvas component.
/// </summary>
public partial class OffcanvasBody : BaseComponent
{
    #region Members

    private int? maxHeight;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        ParentOffcanvas?.NotifyHasOffcanvasBody();
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.OffcanvasBody() );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override void BuildStyles( StyleBuilder builder )
    {
        if ( MaxHeight != null )
            builder.Append( StyleProvider.OffcanvasBodyMaxHeight( MaxHeight ?? 0 ) );

        base.BuildStyles( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Sets the maximum height of the offcanvas body (in viewport size unit).
    /// </summary>
    [Parameter]
    public int? MaxHeight
    {
        get => maxHeight;
        set
        {
            maxHeight = value;

            DirtyStyles();
        }
    }

    /// <summary>
    /// Gets or sets the cascaded parent offcanvas-content component.
    /// </summary>
    [CascadingParameter] protected Offcanvas ParentOffcanvas { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="OffcanvasBody"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}
