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
    #region Methods

    /// <inheritdoc />
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        var definitionChanged = ReportContext is not null
            && ( parameters.IsParameterChanged( Name )
                || parameters.IsParameterChanged( Formula ) );

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
        ReportContext?.UnregisterFormulaField( this );
        ReportContext = null;
    }

    private void UpdateDefinition()
    {
        ReportContext?.RegisterFormulaField( this, new()
        {
            Name = Name,
            Formula = Formula,
        } );
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