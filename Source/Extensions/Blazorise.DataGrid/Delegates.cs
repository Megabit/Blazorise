namespace Blazorise.DataGrid
{
    /// <summary>
    /// Represents the function for the <see cref="DataGrid{TItem}"/> custom filtering.
    /// </summary>
    /// <typeparam name="TItem">Type-parameter of a an object in the DataGrid.</typeparam>
    /// <param name="item">An item in a row for which the filter is executed.</param>
    /// <returns>True it an item has passed the filter operation.</returns>
    public delegate bool DataGridCustomFilter<TItem>( TItem item );

    /// <summary>
    /// Represents the function for the <see cref="DataGridColumn{TItem}"/> custom filtering.
    /// </summary>
    /// <param name="itemValue">An item value in a column cell for which the filter is executed.</param>
    /// <param name="searchValue">Searching value.</param>
    /// <returns>True it the value has passed the filter operation.</returns>
    public delegate bool DataGridColumnCustomFilter( object itemValue, object searchValue );
}