#region Using directives
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Displays report designer validation warnings.
/// </summary>
public partial class _ReportDesignerWarningsDialog
{
    #region Methods

    private Task Close()
        => CloseReportModal();

    #endregion

    #region Properties

    /// <summary>
    /// Validation warnings displayed by the dialog.
    /// </summary>
    [Parameter] public IReadOnlyList<string> Warnings { get; set; } = [];

    #endregion
}