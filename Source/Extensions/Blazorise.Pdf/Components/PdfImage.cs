using Microsoft.AspNetCore.Components;

namespace Blazorise.Pdf;

/// <summary>
/// Defines an image in a PDF document.
/// </summary>
public class PdfImage : BasePdfElement
{
    #region Methods

    /// <inheritdoc />
    protected override void UpdateDefinition( PdfElementDefinition definition )
    {
        base.UpdateDefinition( definition );

        definition.ImageFit = Fit;
    }

    #endregion

    #region Properties

    /// <inheritdoc />
    protected override PdfElementType ElementType => PdfElementType.Image;

    /// <summary>
    /// Defines how the image should fit inside the element bounds.
    /// </summary>
    [Parameter] public PdfImageFit Fit { get; set; } = PdfImageFit.Fill;

    #endregion
}