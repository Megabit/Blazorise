#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Container for an image element.
/// </summary>
public partial class Image : BaseComponent
{
    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.Image() );
        builder.Append( ClassProvider.ImageFluid( Fluid ) );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the string representing the img loading attribute.
    /// </summary>
    protected string LoadingString => Loading ? "lazy" : null;

    /// <summary>
    /// The absolute or relative URL of the image.
    /// </summary>
    [Parameter] public string Source { get; set; }

    /// <summary>
    /// Alternate text for an image.
    /// </summary>
    [Parameter] public string Text { get; set; }

    /// <summary>
    /// Forces an image to take up the whole width.
    /// </summary>
    [Parameter] public bool Fluid { get; set; }

    /// <summary>
    /// Deffers loading the image until it reaches a calculated distance from the viewport.
    /// </summary>
    [Parameter] public bool Loading { get; set; }

    #endregion
}