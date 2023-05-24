#region Using directives
using Blazorise.States;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// The main part of the <see cref="Bar"/>, hidden on touch devices, visible on desktop.
/// </summary>
public partial class BarMenu : BaseComponent
{
    #region Members

    private BarState parentBarState;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.BarMenu( ParentBarState?.Mode ?? BarMode.Horizontal ) );
        builder.Append( ClassProvider.BarMenuShow( ParentBarState?.Mode ?? BarMode.Horizontal ), ParentBarState?.Visible ?? false );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the reference to the parent <see cref="BarMenu"/> component.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Cascaded <see cref="Bar"/> component state object.
    /// </summary>
    [CascadingParameter]
    protected BarState ParentBarState
    {
        get => parentBarState;
        set
        {
            if ( parentBarState == value )
                return;

            parentBarState = value;

            DirtyClasses();
        }
    }

    #endregion
}