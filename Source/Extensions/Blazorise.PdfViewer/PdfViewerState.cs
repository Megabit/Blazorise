#region Using directives
using Blazorise.Infrastructure;
#endregion

namespace Blazorise.PdfViewer;

/// <summary>
/// Manages the state and events for the PDF viewer component.
/// Provides event callbacks for various actions such as initialization, page changes, and scaling.
/// </summary>
public class PdfViewerState
{
    /// <summary>
    /// Gets the event callback that is triggered when the PDF is initialized.
    /// </summary>
    internal EventCallbackSubscribable<PdfModel> PdfInitialized { get; } = new();

    /// <summary>
    /// Gets the event callback that is triggered when the PDF model (e.g., document) is changed.
    /// </summary>
    internal EventCallbackSubscribable<PdfModel> PdfChanged { get; } = new();

    /// <summary>
    /// Gets the event callback that is triggered when a request is made to navigate to the next page.
    /// </summary>
    internal EventCallbackSubscribable<object> NextPageRequested { get; } = new();

    /// <summary>
    /// Gets the event callback that is triggered when a request is made to navigate to the previous page.
    /// </summary>
    internal EventCallbackSubscribable<object> PrevPageRequested { get; } = new();

    /// <summary>
    /// Gets the event callback that is triggered when a request is made to go to a specific page number.
    /// </summary>
    internal EventCallbackSubscribable<int> GoToPageRequested { get; } = new();

    /// <summary>
    /// Gets the event callback that is triggered when a request is made to set the scale of the PDF viewer.
    /// </summary>
    internal EventCallbackSubscribable<double> SetScaleRequested { get; } = new();
}