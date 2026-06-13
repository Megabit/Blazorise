#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Provides a shared base for declarative report data source components.
/// </summary>
public abstract class ReportDataSourceComponentBase : ComponentBase
{
    #region Methods

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        ReportContext?.RegisterDataSource( CreateDataSourceDefinition() );
    }

    /// <summary>
    /// Creates the data source definition registered with the current report.
    /// </summary>
    /// <returns>The data source definition represented by the component parameters.</returns>
    protected abstract ReportDataSourceDefinition CreateDataSourceDefinition();

    #endregion

    #region Properties

    [CascadingParameter] internal ReportContext ReportContext { get; set; }

    #endregion
}