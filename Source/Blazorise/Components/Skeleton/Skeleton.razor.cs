#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Represents a skeleton component used for displaying placeholder content with optional animations.
/// </summary>
public partial class Skeleton : BaseComponent
{
    #region Methods

    /// <summary>
    /// Builds the CSS classes for the skeleton component.
    /// </summary>
    /// <param name="builder">The <see cref="ClassBuilder"/> used to construct the CSS classes.</param>
    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.Skeleton() );
        builder.Append( ClassProvider.SkeletonAnimation( Animation ) );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the animation style applied to the skeleton.
    /// </summary>
    /// <value>
    /// A <see cref="SkeletonAnimation"/> value that determines the animation style.
    /// </value>
    [Parameter] public SkeletonAnimation Animation { get; set; }

    /// <summary>
    /// Gets or sets the child content to be rendered inside the skeleton component.
    /// </summary>
    /// <value>
    /// A <see cref="RenderFragment"/> that represents the content to be displayed within the skeleton.
    /// </value>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}