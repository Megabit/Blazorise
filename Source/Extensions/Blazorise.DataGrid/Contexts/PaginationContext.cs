#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
#endregion

namespace Blazorise.DataGrid;

public class PaginationContext<TItem>
{
    #region Members

    private event PageChangedEventHandler PageChanged;

    public delegate void PageChangedEventHandler( long value );

    private event PageSizeChangedEventHandler PageSizeChanged;

    public delegate void PageSizeChangedEventHandler( int value );

    private event TotalItemsChangedEventHandler TotalItemsChanged;

    public delegate void TotalItemsChangedEventHandler( long value );

    private long firstVisiblePage;

    private long lastVisiblePage;

    private long page = 1;

    private int pageSize = 10;

    private long? totalItems;

    private DataGrid<TItem> parentDataGrid;

    #endregion

    #region Constructors

    public PaginationContext( DataGrid<TItem> parentDataGrid )
    {
        this.parentDataGrid = parentDataGrid;
    }

    #endregion

    #region Methods

    public void SubscribeOnPageChanged( PageChangedEventHandler listener )
    {
        PageChanged += listener;
    }

    public void UnsubscribeOnPageChanged( PageChangedEventHandler listener )
    {
        PageChanged -= listener;
    }

    public void TriggerPageChange( long value )
    {
        PageChanged?.Invoke( value );
    }

    public void SubscribeOnPageSizeChanged( PageSizeChangedEventHandler listener )
    {
        PageSizeChanged += listener;
    }

    public void UnsubscribeOnPageSizeChanged( PageSizeChangedEventHandler listener )
    {
        PageSizeChanged -= listener;
    }

    public void TriggerPageSizeChange( int value )
    {
        PageSizeChanged?.Invoke( value );
    }

    public void SubscribeOnTotalItemsChanged( TotalItemsChangedEventHandler listener )
    {
        TotalItemsChanged += listener;
    }

    public void UnsubscribeOnTotalItemsChanged( TotalItemsChangedEventHandler listener )
    {
        TotalItemsChanged -= listener;
    }

    public void TriggerTotalItemsChange( long value )
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
    public long Page
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
    public long LastPage
    {
        get
        {
            long lastPage = Math.Max( (long)Math.Ceiling( ( TotalItems ?? 0 ) / (double)pageSize ), 1 );

            if ( Page > lastPage )
                Page = lastPage;

            return lastPage;
        }
    }

    /// <summary>
    /// Gets the number of the first page that can be clicked in a large dataset.
    /// </summary>
    public long FirstVisiblePage
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
    public long LastVisiblePage
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

    public bool ShowPageSizes { get; set; } = false;

    public IEnumerable<int> PageSizes { get; set; } = new int[] { 5, 10, 25, 50, 100, 250 };

    public int MaxPaginationLinks { get; set; } = 5;

    /// <summary>
    /// Gets or sets the total number of items. Used only when <see cref="DataGrid{TItem}.ReadData"/> is used to load the data.
    /// </summary>
    /// <remarks>
    /// This field must be set only when <see cref="DataGrid{TItem}.ReadData"/> is used to load the data.
    /// </remarks>
    public long? TotalItems
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