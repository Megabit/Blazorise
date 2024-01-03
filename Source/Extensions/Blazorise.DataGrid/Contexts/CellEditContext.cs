#region Using directives
using System;
#endregion

namespace Blazorise.DataGrid;

/// <summary>
/// Context for editors in datagrid cell.
/// </summary>
public class CellEditContext
{
    protected object cellValue;

    /// <summary>
    /// Gets or sets the editor value.
    /// </summary>
    public object CellValue
    {
        get => cellValue;
        set
        {
            cellValue = value;
            Modified = true;
        }
    }

    public bool Modified { get; private set; }
}

/// <summary>
/// Abstraction of <see cref="CellEditContext"/> that holds the reference to the model being edited.
/// </summary>
/// <typeparam name="TItem"></typeparam>
public class CellEditContext<TItem> : CellEditContext
{
    /// <summary>
    /// Method that will be called when cell is manually updated.
    /// </summary>
    private readonly Action<string, object> SetCellValue;

    /// <summary>
    /// Method that will be called when cell is read.
    /// </summary>
    private readonly Func<string, object> GetCellValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="CellEditContext{TItem}"/>.
    /// </summary>
    /// <param name="item">An item to which this cell belongs.</param>
    /// <param name="cellValue">The initial cell value</param>
    /// <param name="setCellValue">Method that will be called when cell is manually updated.</param>
    /// <param name="getCellValue">Method that will be called when cell value is manually read.</param>
    public CellEditContext( TItem item, object cellValue, Action<string, object> setCellValue, Func<string, object> getCellValue )
    {
        this.Item = item;
        this.cellValue = cellValue;
        this.SetCellValue = setCellValue;
        this.GetCellValue = getCellValue;
    }

    /// <summary>
    /// Gets the reference to the model that is currently in edit mode.
    /// <para>
    /// Note that this model is used only for reading
    /// and you should never update it directly or any of it's field members.
    /// For writing the edited value you must use <see cref="CellEditContext.CellValue"/>.
    /// </para>
    /// </summary>
    public TItem Item { get; }

    /// <summary>
    /// Updated the cell of the current editing item that matches the <paramref name="fieldName"/>.
    /// </summary>
    /// <param name="fieldName">Cell field name.</param>
    /// <param name="value">New cell value.</param>
    public void UpdateCell( string fieldName, object value )
    {
        SetCellValue?.Invoke( fieldName, value );
    }

    /// <summary>
    /// Reads the cell value of the current editing item that matches the <paramref name="fieldName"/>.
    /// </summary>
    /// <param name="fieldName">Cell field name.</param>
    /// <returns>Cell value.</returns>
    public object ReadCell( string fieldName )
    {
        return GetCellValue?.Invoke( fieldName );
    }

    internal void SetCellValueInternal( object cellValue )
    {
        CellValue = cellValue;
    }
}