#region Using directives
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Provides a shared base for declarative report data source components.
/// </summary>
public abstract class BaseReportDataSourceComponent : ComponentBase, IDisposable
{
    #region Members

    private ReportContext registeredReportContext;

    #endregion

    #region Methods

    /// <inheritdoc />
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        bool definitionChanged = registeredReportContext is null || HasDefinitionChanged( parameters );

        await base.SetParametersAsync( parameters );

        bool contextChanged = !ReferenceEquals( registeredReportContext, ReportContext );

        if ( contextChanged )
        {
            registeredReportContext?.UnregisterDataSource( this );
            registeredReportContext = ReportContext;
        }

        if ( definitionChanged || contextChanged )
            registeredReportContext?.RegisterDataSource( this, CreateDataSourceDefinition() );
    }

    /// <inheritdoc />
    public void Dispose()
    {
        registeredReportContext?.UnregisterDataSource( this );
        registeredReportContext = null;
    }

    /// <summary>
    /// Determines whether parameters affecting the data source definition changed.
    /// </summary>
    protected abstract bool HasDefinitionChanged( ParameterView parameters );

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