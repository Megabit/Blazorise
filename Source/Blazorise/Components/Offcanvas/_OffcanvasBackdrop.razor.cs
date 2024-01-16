#region Using directives
using Blazorise.States;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Internal component to render offcanvas backdrop or background.
/// </summary>
public partial class _OffcanvasBackdrop : BaseComponent
{
    #region Members

    private OffcanvasState parentOffcanvasState;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        if ( ParentOffcanvas is not null )
        {
            ParentOffcanvas.NotifyCloseActivatorIdInitialized( ElementId );
        }
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.OffcanvasBackdrop() );
        builder.Append( ClassProvider.OffcanvasBackdropFade( parentOffcanvasState.Showing, parentOffcanvasState.Hiding ) );
        builder.Append( ClassProvider.OffcanvasBackdropVisible( parentOffcanvasState.Visible ) );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override void BuildStyles( StyleBuilder builder )
    {
        builder.Append( StyleProvider.OffcanvasAnimationDuration( Animated, AnimationDuration ) );

        base.BuildStyles( builder );
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    /// <summary>
    /// Gets or sets the cascaded parent offcanvas component.
    /// </summary>
    [CascadingParameter] protected Offcanvas ParentOffcanvas { get; set; }

    /// <summary>
    /// Cascaded <see cref="Offcanvas"/> component state object.
    /// </summary>
    [CascadingParameter]
    protected OffcanvasState ParentOffcanvasState
    {
        get => parentOffcanvasState;
        set
        {
            if ( parentOffcanvasState == value )
                return;

            parentOffcanvasState = value;

            DirtyClasses();
            DirtyStyles();
        }
    }

    /// <summary>
    /// Specifies whether the Offcanvas should have an animated transition.
    /// </summary>
    [Parameter] public bool Animated { get; set; } = true;

    /// <summary>
    /// The duration of the animation in milliseconds.
    /// </summary>
    [Parameter] public int AnimationDuration { get; set; } = 300;

    #endregion
}