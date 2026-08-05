#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Declares a reusable formula-backed field available to report elements and text templates.
/// </summary>
public partial class ReportFormulaField : ComponentBase, IDisposable
{
    #region Members

    private ReportContext registeredReportContext;

    #endregion

    #region Methods

    /// <inheritdoc />
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        bool definitionChanged = registeredReportContext is null
            || parameters.IsParameterChanged( Name )
            || parameters.IsParameterChanged( Formula );

        await base.SetParametersAsync( parameters );

        bool contextChanged = !ReferenceEquals( registeredReportContext, ReportContext );

        if ( contextChanged )
        {
            registeredReportContext?.UnregisterFormulaField( this );
            registeredReportContext = ReportContext;
        }

        if ( definitionChanged || contextChanged )
        {
            registeredReportContext?.RegisterFormulaField( this, new()
            {
                Name = Name,
                Formula = Formula,
            } );
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        registeredReportContext?.UnregisterFormulaField( this );
        registeredReportContext = null;
    }

    #endregion

    #region Properties

    [CascadingParameter] internal ReportContext ReportContext { get; set; }

    /// <summary>
    /// Formula field name shown in the field explorer and used by expressions.
    /// </summary>
    [Parameter] public string Name { get; set; }

    /// <summary>
    /// Formula expression evaluated when the field is rendered.
    /// </summary>
    [Parameter] public string Formula { get; set; }

    #endregion
}