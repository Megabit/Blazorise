#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Declares a reusable formula-backed field available to report elements and text templates.
/// </summary>
public partial class ReportFormulaField : ComponentBase
{
    #region Methods

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        ReportContext?.RegisterFormulaField( new()
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