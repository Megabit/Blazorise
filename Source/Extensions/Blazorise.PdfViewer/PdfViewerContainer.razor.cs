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
    #region Members

    /// <summary>
    /// Provides the state of the <see cref="PdfViewerContainer"/> component.
    /// </summary>
    private PdfViewerState state = new();

    #endregion

    #region Constructors

    /// <summary>
    /// A default <see cref="PdfViewerContainer"/> constructor.
    /// </summary>
    public PdfViewerContainer()
    {
        Background = Background.Light;
        Overflow = Blazorise.Overflow.Auto;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the reference to state object for this <see cref="PdfViewerContainer"/> component.
    /// </summary>
    protected internal PdfViewerState State => state;

    /// <summary>
    /// Gets or sets the content to be rendered inside the component.
    /// </summary>
    /// <remarks>
    /// This property allows developers to define custom content within the PDF container component.
    /// </remarks>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}
