#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Center area of the modal component.
/// </summary>
public partial class ModalBody : BaseComponent
{
    #region Members

    private int? maxHeight;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        ParentModalContent?.NotifyHasModalBody();
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.ModalBody() );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override void BuildStyles( StyleBuilder builder )
    {
        if ( MaxHeight is not null )
            builder.Append( StyleProvider.ModalBodyMaxHeight( MaxHeight ?? 0 ) );

        base.BuildStyles( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Sets the maximum height of the modal body (in viewport size unit).
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
    /// Gets or sets the cascaded parent modal-content component.
    /// </summary>
    [CascadingParameter] protected ModalContent ParentModalContent { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="ModalBody"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}