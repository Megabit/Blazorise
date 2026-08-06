using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;

namespace Blazorise.Pdf;

/// <summary>
/// Defines an image in a PDF document.
/// </summary>
public class PdfImage : BasePdfElement
{
    #region Methods

    /// <inheritdoc />
    protected override bool IsDefinitionChanged( ParameterView parameters )
    {
        return base.IsDefinitionChanged( parameters )
            || parameters.IsParameterChanged( Source )
            || parameters.IsParameterChanged( ClipContent )
            || parameters.IsParameterChanged( Fit )
            || parameters.IsParameterChanged( BackgroundColor );
    }

    /// <inheritdoc />
    protected override void UpdateDefinition( PdfElementDefinition definition )
    {
        base.UpdateDefinition( definition );

        definition.Source = Source;
        definition.ImageFit = Fit;
    }

    #endregion

    #region Properties

    /// <inheritdoc />
    protected override PdfElementType ElementType => PdfElementType.Image;

    /// <inheritdoc />
    protected override bool ElementClipContent => ClipContent;

    /// <inheritdoc />
    protected override string ElementBackgroundColor => BackgroundColor;

    /// <summary>
    /// Image source resolved by <see cref="IPdfResourceResolver"/>. The default resolver accepts base64 data URIs.
    /// </summary>
    [Parameter] public string Source { get; set; }

    /// <summary>
    /// Indicates that content should be clipped to the element bounds.
    /// </summary>
    [Parameter] public bool ClipContent { get; set; } = true;

    /// <summary>
    /// Defines how the image should fit inside the element bounds.
    /// </summary>
    [Parameter] public PdfImageFit Fit { get; set; } = PdfImageFit.Fill;

    /// <summary>
    /// Background color in hexadecimal format.
    /// </summary>
    [Parameter] public string BackgroundColor { get; set; }

    #endregion
}