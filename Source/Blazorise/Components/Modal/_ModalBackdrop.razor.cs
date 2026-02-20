#region Using directives
using Blazorise.Extensions;
using Blazorise.States;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Internal component to render modal backdrop or background.
/// </summary>
public partial class _ModalBackdrop : BaseComponent
{
    #region Members

    private ModalState parentModalState;

    private string backdropClass;

    private string backdropStyle;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        if ( ParentModal is not null )
        {
            ParentModal.NotifyCloseActivatorIdInitialized( ElementId );
        }
    }

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        var nextBackdropClass = ParentModal?.Classes?.Backdrop;

        if ( !nextBackdropClass.IsEqual( backdropClass ) )
        {
            backdropClass = nextBackdropClass;
            DirtyClasses();
        }

        var nextBackdropStyle = ParentModal?.Styles?.Backdrop;

        if ( !nextBackdropStyle.IsEqual( backdropStyle ) )
        {
            backdropStyle = nextBackdropStyle;
            DirtyStyles();
        }
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.ModalBackdrop() );
        builder.Append( ClassProvider.ModalBackdropFade() );
        builder.Append( ClassProvider.ModalBackdropVisible( parentModalState.Visible ) );
        builder.Append( ParentModal?.Classes?.Backdrop );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override void BuildStyles( StyleBuilder builder )
    {
        builder.Append( StyleProvider.ModalBackdropZIndex( parentModalState.OpenIndex ) );
        builder.Append( $"--modal-animation-duration: {( Animated ? AnimationDuration : 0 )}ms" );
        builder.Append( ParentModal?.Styles?.Backdrop );

        base.BuildStyles( builder );
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    /// <summary>
    /// Gets or sets whether the component has any animations.
    /// </summary>
    [Parameter] public bool Animated { get; set; } = true;

    /// <summary>
    /// Gets or sets the animation duration.
    /// </summary>
    [Parameter] public int AnimationDuration { get; set; } = 150;

    /// <summary>
    /// Gets or sets the cascaded parent modal component.
    /// </summary>
    [CascadingParameter] protected Modal ParentModal { get; set; }

    /// <summary>
    /// Cascaded <see cref="Modal"/> component state object.
    /// </summary>
    [CascadingParameter]
    protected ModalState ParentModalState
    {
        get => parentModalState;
        set
        {
            if ( parentModalState == value )
                return;

            parentModalState = value;

            DirtyClasses();
            DirtyStyles();
        }
    }

    #endregion
}