#region Using directives
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Pdf;

/// <summary>
/// Defines a table layout in a PDF document.
/// </summary>
public partial class PdfTable : BasePdfElement
{
    #region Members

    private PdfTableContext tableContext;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new PDF table.
    /// </summary>
    public PdfTable()
    {
        BorderWidth = 1;
    }

    #endregion

    #region Methods

    /// <inheritdoc />
    protected override bool IsDefinitionChanged( ParameterView parameters )
    {
        return base.IsDefinitionChanged( parameters )
            || parameters.IsParameterChanged( ClipContent )
            || parameters.IsParameterChanged( BackgroundColor );
    }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();
        tableContext = new( Definition );
    }

    #endregion

    #region Properties

    /// <inheritdoc />
    protected override PdfElementType ElementType => PdfElementType.Table;

    /// <inheritdoc />
    protected override bool ElementClipContent => ClipContent;

    /// <inheritdoc />
    protected override string ElementBackgroundColor => BackgroundColor;

    /// <summary>
    /// Indicates that content should be clipped to the element bounds.
    /// </summary>
    [Parameter] public bool ClipContent { get; set; } = true;

    /// <summary>
    /// Background color in hexadecimal format.
    /// </summary>
    [Parameter] public string BackgroundColor { get; set; }

    /// <summary>
    /// Rows declared inside the table.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}