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
    #region Methods

    /// <inheritdoc />
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        var definitionChanged = ReportContext is not null && HasDefinitionChanged( parameters );

        await base.SetParametersAsync( parameters );

        if ( definitionChanged )
        {
            UpdateDefinition();
        }
    }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();

        UpdateDefinition();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        ReportContext?.UnregisterDataSource( this );
        ReportContext = null;
    }

    private void UpdateDefinition()
    {
        ReportContext?.RegisterDataSource( this, CreateDataSourceDefinition() );
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