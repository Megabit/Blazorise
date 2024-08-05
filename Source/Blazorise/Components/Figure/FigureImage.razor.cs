#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Wrapper for a <see cref="Figure"/> image.
/// </summary>
public partial class FigureImage : BaseComponent
{
    #region Members

    private bool rounded;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.FigureImage() );
        builder.Append( ClassProvider.FigureImageRounded( Rounded ) );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// The absolute or relative URL of the image.
    /// </summary>
    [Parameter] public string Source { get; set; }

    /// <summary>
    /// Alternate text for an image.
    /// </summary>
    [Parameter] public string AlternateText { get; set; }

    /// <summary>
    /// True if container should have a rounded corners.
    /// </summary>
    [Parameter]
    public bool Rounded
    {
        get => rounded;
        set
        {
            rounded = value;

            DirtyClasses();
        }
    }

    #endregion
}