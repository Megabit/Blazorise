using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;

namespace Blazorise.Pdf;

/// <summary>
/// Defines a rectangle in a PDF document.
/// </summary>
public class PdfRectangle : BasePdfElement
{
    #region Constructors

    /// <summary>
    /// Initializes a new PDF rectangle.
    /// </summary>
    public PdfRectangle()
    {
        BorderWidth = 1;
    }

    #endregion

    #region Methods

    /// <inheritdoc />
    protected override bool IsDefinitionChanged( ParameterView parameters )
    {
        return base.IsDefinitionChanged( parameters )
            || parameters.IsParameterChanged( BackgroundColor );
    }

    #endregion

    #region Properties

    /// <inheritdoc />
    protected override PdfElementType ElementType => PdfElementType.Rectangle;

    /// <inheritdoc />
    protected override string ElementBackgroundColor => BackgroundColor;

    /// <summary>
    /// Background color in hexadecimal format.
    /// </summary>
    [Parameter] public string BackgroundColor { get; set; }

    #endregion
}