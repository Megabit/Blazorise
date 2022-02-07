namespace Blazorise.DataGrid
{
    /// <summary>
    /// Context for editors in datagrid filter section.
    /// </summary>
    public class FilterContext<TItem>
    {
        #region Members

        private event FilterChangedEventHandler SearchValueChanged;

        public delegate void FilterChangedEventHandler( object value );

        #endregion

        #region Methods

        public void Subscribe( FilterChangedEventHandler listener )
        {
            SearchValueChanged += listener;
        }

        public void Unsubscribe( FilterChangedEventHandler listener )
        {
            SearchValueChanged -= listener;
        }

        public void TriggerFilterChange( object value )
        {
            SearchValue = value;
            SearchValueChanged?.Invoke( value );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the filter value.
        /// </summary>
        public object SearchValue { get; set; }

        #endregion
    }
}