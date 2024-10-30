namespace Blazorise.DataGrid;

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

    public void SetSearchValue1( object value1 )
    {
        SearchValues ??= new object[2];
        SearchValues[0] = value1;
    }

    public void SetSearchValue2( object value2 )
    {
        SearchValues ??= new object[2];
        SearchValues[1] = value2;
    }

    public object GetSearchValue1( )
    {
        return SearchValues?[0];
    }

    public object GetSearchValue2( )
    {
        return SearchValues?[1];
    }
    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the filter value.
    /// </summary>
    public object SearchValue { get; set; }

    /// <summary>
    /// Gets or sets the filter value(s).
    /// <para>Represents the value(s) for filter methods that are range based.</para>
    /// </summary>
    public object[] SearchValues { get; set; }

    #endregion
}