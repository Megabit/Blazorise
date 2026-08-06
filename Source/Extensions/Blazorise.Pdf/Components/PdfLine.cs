using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;

namespace Blazorise.Pdf;

/// <summary>
/// Defines a line in a PDF document.
/// </summary>
public class PdfLine : BasePdfElement
{
    #region Constructors

    /// <summary>
    /// Initializes a new PDF line.
    /// </summary>
    public PdfLine()
    {
        BorderWidth = 1;
    }

    #endregion

    #region Methods

    /// <inheritdoc />
    protected override bool IsDefinitionChanged( ParameterView parameters )
    {
        return base.IsDefinitionChanged( parameters )
            || parameters.IsParameterChanged( Orientation );
    }

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