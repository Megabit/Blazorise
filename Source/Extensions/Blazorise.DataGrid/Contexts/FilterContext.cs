namespace Blazorise.DataGrid;

/// <summary>
/// Context for editors in datagrid filter section.
/// </summary>
public class FilterContext<TItem>
{
    #region Members

    private event FilterChangedEventHandler SearchValueChanged;

    /// <summary>
    /// Handles notifications for filter changed event handler.
    /// </summary>
    public delegate void FilterChangedEventHandler( object value );

    #endregion

    #region Methods

    /// <summary>
    /// Registers a listener for filter-value changes.
    /// </summary>
    public void Subscribe( FilterChangedEventHandler listener )
    {
        SearchValueChanged += listener;
    }

    /// <summary>
    /// Removes a filter-value change listener.
    /// </summary>
    public void Unsubscribe( FilterChangedEventHandler listener )
    {
        SearchValueChanged -= listener;
    }

    /// <summary>
    /// Notifies listeners of filter change.
    /// </summary>
    public void TriggerFilterChange( object value )
    {
        SearchValue = value;
        SearchValueChanged?.Invoke( value );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the filter value(s). 
    /// <para>Reprents a single value OR</para>
    /// <para>Represents the value(s) for filter methods that are range based.</para>
    /// </summary>
    public object SearchValue { get; set; }

    #endregion
}