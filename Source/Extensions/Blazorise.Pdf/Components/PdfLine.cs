using Microsoft.AspNetCore.Components;

namespace Blazorise.Pdf;

/// <summary>
/// Defines a line in a PDF document.
/// </summary>
public class PdfLine : BasePdfElement
{
    #region Methods

    /// <inheritdoc />
    protected override void UpdateDefinition( PdfElementDefinition definition )
    {
        base.UpdateDefinition( definition );

        definition.Orientation = Orientation;
    }

    #endregion

    #region Properties

    /// <inheritdoc />
    protected override PdfElementType ElementType => PdfElementType.Line;

    /// <summary>
    /// Line orientation within the element bounds.
    /// </summary>
    [Parameter] public Orientation Orientation { get; set; } = Orientation.Horizontal;

    #endregion
}