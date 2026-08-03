#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
#endregion

namespace Blazorise.DataGrid;

/// <summary>
/// Carries pagination state and callbacks.
/// </summary>
public class PaginationContext<TItem>
{
    #region Members

    private event PageChangedEventHandler PageChanged;

    /// <summary>
    /// Handles notifications for page changed event handler.
    /// </summary>
    public delegate void PageChangedEventHandler( int value );

    private event PageSizeChangedEventHandler PageSizeChanged;

    /// <summary>
    /// Handles notifications for page size changed event handler.
    /// </summary>
    public delegate void PageSizeChangedEventHandler( int value );

    private event TotalItemsChangedEventHandler TotalItemsChanged;

    /// <summary>
    /// Handles notifications for total items changed event handler.
    /// </summary>
    public delegate void TotalItemsChangedEventHandler( int value );

    private int firstVisiblePage;

    private int lastVisiblePage;

    private int page = 1;

    private int pageSize = 10;

    private int? totalItems;

    private DataGrid<TItem> parentDataGrid;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a pagination context instance.
    /// </summary>
    public PaginationContext( DataGrid<TItem> parentDataGrid )
    {
        this.parentDataGrid = parentDataGrid;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Subscribes a listener to page changed notifications.
    /// </summary>
    public void SubscribeOnPageChanged( PageChangedEventHandler listener )
    {
        PageChanged += listener;
    }

    /// <summary>
    /// Removes a listener from page changed notifications.
    /// </summary>
    public void UnsubscribeOnPageChanged( PageChangedEventHandler listener )
    {
        PageChanged -= listener;
    }

    /// <summary>
    /// Notifies listeners of page change.
    /// </summary>
    public void TriggerPageChange( int value )
    {
        PageChanged?.Invoke( value );
    }

    /// <summary>
    /// Subscribes a listener to page size changed notifications.
    /// </summary>
    public void SubscribeOnPageSizeChanged( PageSizeChangedEventHandler listener )
    {
        PageSizeChanged += listener;
    }

    /// <summary>
    /// Removes a listener from page size changed notifications.
    /// </summary>
    public void UnsubscribeOnPageSizeChanged( PageSizeChangedEventHandler listener )
    {
        PageSizeChanged -= listener;
    }

    /// <summary>
    /// Notifies listeners of page size change.
    /// </summary>
    public void TriggerPageSizeChange( int value )
    {
        PageSizeChanged?.Invoke( value );
    }

    /// <summary>
    /// Subscribes a listener to total items changed notifications.
    /// </summary>
    public void SubscribeOnTotalItemsChanged( TotalItemsChangedEventHandler listener )
    {
        TotalItemsChanged += listener;
    }

    /// <summary>
    /// Removes a listener from total items changed notifications.
    /// </summary>
    public void UnsubscribeOnTotalItemsChanged( TotalItemsChangedEventHandler listener )
    {
        TotalItemsChanged -= listener;
    }

    /// <summary>
    /// Notifies listeners of total items change.
    /// </summary>
    public void TriggerTotalItemsChange( int value )
    {
        TotalItemsChanged?.Invoke( value );
    }

    /// <summary>
    /// Calculates the first and last visible pages based on the current offset and page size.
    /// </summary>
    private void CalculateFirstAndLastVisiblePage()
    {
        var step = (int)Math.Floor( MaxPaginationLinks / 2d );

        var leftButton = Page - step;
        var rightButton = Page + step;

        if ( leftButton <= 1 )
        {
            firstVisiblePage = 1;
            lastVisiblePage = Math.Min( MaxPaginationLinks, LastPage );
        }
        else if ( LastPage <= rightButton )
        {
            firstVisiblePage = Math.Max( LastPage - MaxPaginationLinks + 1, 1 );
            lastVisiblePage = LastPage;
        }
        else
        {
            firstVisiblePage = leftButton;
            lastVisiblePage = rightButton;
        }
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the CancellationTokenSource which could be used to issue a cancellation.
    /// </summary>
    public CancellationTokenSource CancellationTokenSource { get; set; }

    /// <summary>
    /// Gets or sets the current page
    /// </summary>
    public int Page
    {
        get => page;
        set
        {
            if ( page != value )
            {
                page = value;
                TriggerPageChange( value );
            }
        }
    }

    /// <summary>
    /// Gets the last page number.
    /// </summary>
    public int LastPage
    {
        get
        {
            var lastPage = Math.Max( (int)Math.Ceiling( ( TotalItems ?? 0 ) / (double)pageSize ), 1 );

            if ( Page > lastPage )
                Page = lastPage;

            return lastPage;
        }
    }

    /// <summary>
    /// Gets the number of the first page that can be clicked in a large dataset.
    /// </summary>
    public int FirstVisiblePage
    {
        get
        {
            CalculateFirstAndLastVisiblePage();

            return firstVisiblePage;
        }
    }

    /// <summary>
    /// Gets the number of the last page that can be clicked in a large dataset.
    /// </summary>
    public int LastVisiblePage
    {
        get
        {
            CalculateFirstAndLastVisiblePage();

            return lastVisiblePage;
        }
    }

    /// <summary>
    /// Gets or sets the current page size
    /// </summary>
    public int PageSize
    {
        get => pageSize;
        set
        {
            if ( pageSize != value )
            {
                pageSize = value;
                TriggerPageSizeChange( value );
            }
        }
    }

    /// <summary>
    /// Controls whether page sizes is shown.
    /// </summary>
    public bool ShowPageSizes { get; set; } = false;

    /// <summary>
    /// Page-size choices offered by the paginator.
    /// </summary>
    public IEnumerable<int> PageSizes { get; set; } = new int[] { 5, 10, 25, 50, 100, 250 };

    /// <summary>
    /// Maximum number of numbered page links to render.
    /// </summary>
    public int MaxPaginationLinks { get; set; } = 5;

    /// <summary>
    /// Gets or sets the total number of items. Used only when <see cref="DataGrid{TItem}.ReadData"/> is used to load the data.
    /// </summary>
    /// <remarks>
    /// This field must be set only when <see cref="DataGrid{TItem}.ReadData"/> is used to load the data.
    /// </remarks>
    public int? TotalItems
    {
        // If we're using ReadData than TotalItems must be set so we can know how many items are available
        get => ( ( parentDataGrid.ManualReadMode || parentDataGrid.VirtualizeManualReadMode ) ? totalItems : parentDataGrid.FilteredData?.Count() ) ?? 0;
        set
        {
            if ( totalItems != value )
            {
                totalItems = value;

                TriggerTotalItemsChange( value ?? default );
            }
        }
    }

    #endregion
}