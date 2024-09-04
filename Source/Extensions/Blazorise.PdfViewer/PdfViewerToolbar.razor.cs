#region Using directives
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise.Infrastructure;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.PdfViewer;

/// <summary>
/// Represents a toolbar component for interacting with the PDF viewer.
/// Provides controls for navigation, zooming, and other PDF-related actions.
/// </summary>
public partial class PdfViewerToolbar : BaseComponent, IDisposable
{
    #region Members

    private readonly EventCallbackSubscriber<PdfModel> pdfInitializedSubscriber;
    private readonly EventCallbackSubscriber<PdfModel> pdfChangedSubscriber;

    private int totalPages;

    private int pageNumber = 1;

    private readonly List<int> zoomLevels = new()
    {
        50,
        75,
        100,
        125,
        150,
        200,
        300,
        400,
        500,
    };

    private int zoomLevelIndex = 2;

    private double zoomLevelPercentage = 100;

    #endregion

    #region Constructors

    /// <summary>
    /// A default <see cref="PdfViewerToolbar"/> constructor.
    /// </summary>
    public PdfViewerToolbar()
    {
        Background = Background.White;
        Flex = Blazorise.Flex.JustifyContent.Between;
        Padding = Blazorise.Padding.Is2;
        Position = Blazorise.Position.Sticky.Top;
        pdfInitializedSubscriber = new EventCallbackSubscriber<PdfModel>( EventCallback.Factory.Create<PdfModel>( this, OnPdfInitialized ) );
        pdfChangedSubscriber = new EventCallbackSubscriber<PdfModel>( EventCallback.Factory.Create<PdfModel>( this, OnPdfChanged ) );
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override Task OnParametersSetAsync()
    {
        pdfInitializedSubscriber.SubscribeOrMove( ViewerState?.PdfInitialized );
        pdfChangedSubscriber.SubscribeOrMove( ViewerState?.PdfChanged );

        return base.OnParametersSetAsync();
    }

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        if ( disposing )
        {
            pdfInitializedSubscriber?.Dispose();
            pdfChangedSubscriber?.Dispose();
        }

        base.Dispose( disposing );
    }

    private Task OnPdfInitialized( PdfModel model )
    {
        pageNumber = model.PageNumber;
        totalPages = model.TotalPages;

        return Task.CompletedTask;
    }

    private Task OnPdfChanged( PdfModel model )
    {
        pageNumber = model.PageNumber;
        totalPages = model.TotalPages;

        zoomLevelPercentage = model.Scale * 100.0;
        zoomLevelIndex = zoomLevels.IndexOf( (int)zoomLevelPercentage );

        return Task.CompletedTask;
    }

    private async Task OnPreviousPageClicked()
    {
        if ( ViewerState?.PrevPageRequested is not null )
        {
            await ViewerState.PrevPageRequested.InvokeCallbackAsync();
        }
    }

    private async Task OnNextPageClicked()
    {
        if ( ViewerState?.NextPageRequested is not null )
        {
            await ViewerState.NextPageRequested.InvokeCallbackAsync();
        }
    }

    private async Task OnPageNumberChanged( int value )
    {
        if ( value < 1 )
            pageNumber = 1;
        else if ( value > totalPages )
            pageNumber = totalPages;
        else
            pageNumber = value;

        if ( ViewerState?.GoToPageRequested is not null )
        {
            await ViewerState.GoToPageRequested.InvokeCallbackAsync( pageNumber );
        }
    }

    private async Task OnZoomInClicked()
    {
        if ( zoomLevelIndex >= zoomLevels.Count - 1 )
            return;

        zoomLevelIndex++;
        if ( ViewerState?.SetScaleRequested is not null )
        {
            await ViewerState.SetScaleRequested.InvokeCallbackAsync( zoomLevels[zoomLevelIndex] / 100.0 );
        }
    }

    private async Task OnZoomOutClicked()
    {
        if ( zoomLevelIndex <= 0 )
            return;

        zoomLevelIndex--;
        if ( ViewerState?.SetScaleRequested is not null )
        {
            await ViewerState.SetScaleRequested.InvokeCallbackAsync( zoomLevels[zoomLevelIndex] / 100.0 );
        }
    }

    private async Task OnPrintClicked()
    {
        if ( ViewerState?.PrintRequested is not null )
        {
            await ViewerState.PrintRequested.InvokeCallbackAsync();
        }
    }

    #endregion

    #region Properties

    /// <summary>
    /// Defines if the toolbar should be displayed.
    /// </summary>
    protected bool ShowToolbar => ShowPaging || ShowZooming || ShowPrinting;

    /// <summary>
    /// Gets or sets the viewer state.
    /// </summary>
    [CascadingParameter] public PdfViewerState ViewerState { get; set; }

    /// <summary>
    /// Defines if the paging buttons should be displayed.
    /// </summary>
    [Parameter] public bool ShowPaging { get; set; } = true;

    /// <summary>
    /// Defines if the zoom buttons should be displayed.
    /// </summary>
    [Parameter] public bool ShowZooming { get; set; } = true;

    /// <summary>
    /// Defines if the print button should be displayed.
    /// </summary>
    [Parameter] public bool ShowPrinting { get; set; } = true;

    #endregion
}
