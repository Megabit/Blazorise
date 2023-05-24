#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Wrapper for buttons, labels or inputs placed inside of <see cref="Addons"/> component.
/// </summary>
public partial class Addon : BaseComponent
{
    #region Members

    private AddonType addonType = AddonType.Body;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.Addon( AddonType ) );
        builder.Append( ClassProvider.AddonSize( ParentAddons?.Size ?? Size.Default ) );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Defines the location and behaviour of addon container.
    /// </summary>
    [Parameter]
    public AddonType AddonType
    {
        get => addonType;
        set
        {
            addonType = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="Accordion"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the reference to the parent addons.
    /// </summary>
    [CascadingParameter] protected Addons ParentAddons { get; set; }

    #endregion
}