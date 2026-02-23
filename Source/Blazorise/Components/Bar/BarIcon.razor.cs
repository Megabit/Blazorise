#region Using directives
using Blazorise.States;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// A wrapper component around <see cref="Icon"/> that is used by the <see cref="Bar"/> component.
/// </summary>
public partial class BarIcon : BaseComponent
{
    #region Members

    private BarState parentBarState;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        BuildBarIconClasses( builder );

        base.BuildClasses( builder );
    }

    /// <summary>
    /// Builds class names for bar icon.
    /// </summary>
    /// <param name="builder">Class builder.</param>
    protected virtual void BuildBarIconClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.BarIcon( ParentBarState?.Mode ?? BarMode.Horizontal ) );
    }

    #endregion

    #region Properties

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

    /// <summary>
    /// Icon name that can be either a string or <see cref="IconName"/>.
    /// </summary>
    [Parameter] public object IconName { get; set; }

    /// <summary>
    /// Suggested icon style.
    /// </summary>
    [Parameter] public IconStyle IconStyle { get; set; }

    /// <summary>
    /// Defines the icon size.
    /// </summary>
    [Parameter] public IconSize IconSize { get; set; }

    #endregion
}