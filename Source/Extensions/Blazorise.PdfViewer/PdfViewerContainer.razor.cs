#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.PdfViewer;

/// <summary>
/// Represents a container component for the PDF viewer.
/// This class serves as a wrapper or layout for displaying the PDF viewer and related controls.
/// </summary>
public partial class PdfViewerContainer : BaseComponent
{
    #region Constructors

    /// <summary>
    /// A default <see cref="PdfViewerContainer"/> constructor.
    /// </summary>
    public PdfViewerContainer()
    {
        Background = Background.Light;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the content to be rendered inside the component.
    /// </summary>
    /// <remarks>
    /// This property allows developers to define custom content within the PDF container component.
    /// </remarks>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}
