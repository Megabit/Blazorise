#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Internal editor for DataSet and DataTable data source settings.
/// </summary>
public partial class _ReportDataSetDataSourceEditor
{
    #region Methods

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        TableName = Context?.GetString( DataSetReportDataSourceProvider.TableNameSetting );
    }

    private Task OnTableNameChanged( string value )
    {
        TableName = value;
        Context?.SetValue( DataSetReportDataSourceProvider.TableNameSetting, string.IsNullOrWhiteSpace( value ) ? null : value );

        return Task.CompletedTask;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Provider settings context edited by the DataSet data source editor.
    /// </summary>
    [Parameter] public ReportDataSourceProviderEditorContext Context { get; set; }

    private string TableName { get; set; }

    #endregion
}