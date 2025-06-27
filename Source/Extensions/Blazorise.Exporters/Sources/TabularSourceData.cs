using System.Collections.Generic;

namespace Blazorise.Exporters;

/// <summary>
/// Represents a data structure for tabular data, including rows and column names.
/// </summary>
/// <typeparam name="TDataType">Specifies the type of data stored in each cell of the table.</typeparam>
public class TabularSourceData<TDataType> : IExportableData<TDataType>
{
    /// <summary>
    /// Holds a collection of lists, each containing elements of type TDataType. It is initialized as an empty list.
    /// </summary>
    public List<List<TDataType>> Data { get; set; } = new();

    /// <summary>
    /// A list that holds the names of columns. It is initialized as an empty list.
    /// </summary>
    public List<string> ColumnNames { get; set; } = new();
}