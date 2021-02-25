namespace Blazorise.DataGrid
{
    /// <summary>
    /// Context for editors in datagrid filter section.
    /// </summary>
    public class FilterContext
    {
        #region Members

        private event FilterChangedEventHandler SearchValueChanged;

        public delegate void FilterChangedEventHandler( string value );

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

        public void TriggerFilterChange( string value )
        {
            SearchValue = value;
            SearchValueChanged?.Invoke( value );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the filter value.
        /// </summary>
        public string SearchValue { get; set; }

        #endregion
    }
}
