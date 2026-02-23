#region Using directives
using Blazorise.States;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Part of the <see cref="Bar"/> component that is always visible, and which usually contains
/// the logo and optionally some links or icons.
/// </summary>
public partial class BarBrand : BaseComponent
{
    #region Members

    private BarState parentBarState;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.BarBrand( ParentBarState?.Mode ?? BarMode.Horizontal ) );

        base.BuildClasses( builder );
    }

    /// <summary>
    /// Builds class names for bar toggler rendered within brand section.
    /// </summary>
    /// <param name="builder">Class builder.</param>
    protected virtual void BuildBarBrandTogglerClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.BarBrandToggler( ParentBarState?.Mode ?? BarMode.Horizontal ) );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="BarDropdownItem"/>.
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

    /// <summary>
    /// Gets the class names for bar toggler rendered within brand section.
    /// </summary>
    protected string BarBrandTogglerClassNames
    {
        get
        {
            var builder = new ClassBuilder( BuildBarBrandTogglerClasses );

            return builder.Class;
        }
    }

    #endregion
}