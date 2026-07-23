#region Using directives
using System.Collections.Generic;
using Blazorise.Reporting;
#endregion

namespace Blazorise.Reporting.DataSources.Csv;

internal sealed class CsvDataSourceTable
{
    #region Constructors

    internal CsvDataSourceTable( List<Dictionary<string, object>> rows, ReportDataSourceSchema schema )
    {
        Rows = rows;
        Schema = schema;
    }

    #endregion

    #region Properties

    internal List<Dictionary<string, object>> Rows { get; }

    internal ReportDataSourceSchema Schema { get; }

    #endregion
}