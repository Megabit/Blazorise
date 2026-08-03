#region Using directives
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Displays report designer coordinates, warnings, and operation progress.
/// </summary>
public partial class _ReportDesignerStatusBar
{
    #region Members

    private IReadOnlyList<ReportDesignerWarning> warnings = [];

    private ReportProgress progress;

    #endregion

    #region Methods

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        Designer.Progressed += OnProgressed;
        Designer.OperationFinished += OnOperationFinished;
        Designer.WarningsChanged += OnWarningsChanged;

        base.OnInitialized();
    }

    /// <inheritdoc />
    protected override void Dispose( bool disposing )
    {
        if ( disposing )
        {
            Designer.Progressed -= OnProgressed;
            Designer.OperationFinished -= OnOperationFinished;
            Designer.WarningsChanged -= OnWarningsChanged;
        }

        base.Dispose( disposing );
    }

    private void OnProgressed( ReportProgress value )
    {
        progress = value;
        _ = InvokeAsync( StateHasChanged );
    }

    private void OnOperationFinished()
    {
        progress = null;
        _ = InvokeAsync( StateHasChanged );
    }

    private void OnWarningsChanged()
    {
        warnings = Designer.GetDesignerWarnings();
        _ = InvokeAsync( StateHasChanged );
    }

    private Task ShowWarnings()
    {
        return ShowReportModal<_ReportDesignerWarningsDialog>( parameters =>
        {
            parameters.Add( nameof( _ReportDesignerWarningsDialog.Warnings ), warnings.Select( warning => warning.Message ).ToList() );
        }, CreateReportModalOptions( ModalSize.Large ) );
    }

    #endregion

    #region Properties

    private int? Percentage => progress?.Percentage is double value
        ? (int)value
        : null;

    private string WarningsTitle => warnings.Count == 1
        ? "1 report warning"
        : $"{warnings.Count} report warnings";

    /// <summary>
    /// Designer whose status is presented.
    /// </summary>
    [Parameter, EditorRequired] public _ReportDesigner Designer { get; set; }

    #endregion
}