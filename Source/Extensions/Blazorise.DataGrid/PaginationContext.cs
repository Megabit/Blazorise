﻿#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#endregion

namespace Blazorise.DataGrid
{
    public class PaginationContext<TItem>
    {
        #region Members

        private event CurrentPageChangedEventHandler CurrentPageChanged;

        public delegate void CurrentPageChangedEventHandler( int value );

        private event CurrentPageSizeChangedEventHandler CurrentPageSizeChanged;

        public delegate void CurrentPageSizeChangedEventHandler( int value );

        private event TotalItemsChangedEventHandler TotalItemsChanged;

        public delegate void TotalItemsChangedEventHandler( int value );

        private int firstVisiblePage;

        private int lastVisiblePage;

        private int currentPage = 1;

        private int currentPageSize = 5;

        private int? totalItems;

        private DataGrid<TItem> parentDataGrid;

        #endregion

        #region Constructors

        public PaginationContext( DataGrid<TItem> parentDataGrid )
        {
            this.parentDataGrid = parentDataGrid;
        }

        #endregion

        #region Methods

        public void SubscribeOnPageChanged( CurrentPageChangedEventHandler listener )
        {
            CurrentPageChanged += listener;
        }

        public void UnsubscribeOnPageChanged( CurrentPageChangedEventHandler listener )
        {
            CurrentPageChanged -= listener;
        }

        public void TriggerCurrentPageChange( int value )
        {
            CurrentPageChanged?.Invoke( value );
        }

        public void SubscribeOnPageSizeChanged( CurrentPageSizeChangedEventHandler listener )
        {
            CurrentPageSizeChanged += listener;
        }

        public void UnsubscribeOnPageSizeChanged( CurrentPageSizeChangedEventHandler listener )
        {
            CurrentPageSizeChanged -= listener;
        }

        public void TriggerCurrentPageSizeChange( int value )
        {
            CurrentPageSizeChanged?.Invoke( value );
        }

        public void SubscribeOnTotalItemsChanged( TotalItemsChangedEventHandler listener )
        {
            TotalItemsChanged += listener;
        }

        public void UnsubscribeOnTotalItemsChanged( TotalItemsChangedEventHandler listener )
        {
            TotalItemsChanged -= listener;
        }

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

            var leftButton = CurrentPage - step;
            var rightButton = CurrentPage + step;

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
        public int CurrentPage
        {
            get => currentPage;
            set
            {
                if ( currentPage != value )
                {
                    currentPage = value;
                    TriggerCurrentPageChange( value );
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

                var lastPage = Math.Max( (int)Math.Ceiling( ( TotalItems ?? 0 ) / (double)currentPageSize ), 1 );

                if ( CurrentPage > lastPage )
                    CurrentPage = lastPage;

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
        public int CurrentPageSize
        {
            get => currentPageSize;
            set
            {
                if ( currentPageSize != value )
                {
                    currentPageSize = value;
                    TriggerCurrentPageSizeChange( value );
                }
            }
        }

        public bool ShowPageSizes { get; set; } = false;

        public IEnumerable<int> PageSizes { get; set; } = new int[] { 5, 10, 25, 50, 100, 250 };

        public int MaxPaginationLinks { get; set; } = 5;

        /// <summary>
        /// Gets or sets the total number of items. Used only when <see cref="ReadData"/> is used to load the data.
        /// </summary>
        /// <remarks>
        /// This field must be set only when <see cref="ReadData"/> is used to load the data.
        /// </remarks>
        public int? TotalItems
        {
            // If we're using ReadData than TotalItems must be set so we can know how many items are available
            get => ( parentDataGrid.ManualReadMode ? totalItems : parentDataGrid.FilteredData?.Count() ) ?? 0;
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
}
