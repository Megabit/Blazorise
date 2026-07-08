#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Internal dialog used to name a report formula field before editing its formula.
/// </summary>
public partial class _ReportDesignerFormulaFieldNameDialog
{
    #region Members

    private string name;

    private string title;

    #endregion

    #region Methods

    internal async Task Show( string value )
    {
        await Show( value, null );
    }

    internal async Task Show( string value, string title )
    {
        await ShowReportModal<_ReportDesignerFormulaFieldNameDialog>( parameters =>
        {
            parameters.Add( nameof( InitialValue ), value );
            parameters.Add( nameof( InitialTitle ), title );
            parameters.Add( nameof( Confirmed ), Confirmed );
        } );
    }

    private Task Close()
    {
        return CloseReportModal();
    }

    private async Task Confirm()
    {
        if ( !CanConfirm )
            return;

        string confirmedName = name.Trim();

        await CloseReportModal();
        await Confirmed.InvokeAsync( confirmedName );
    }

    private Task NameChanged( string value )
    {
        name = value;

        return Task.CompletedTask;
    }

    #endregion

    #region Properties

    [Parameter] public string InitialValue { get; set; }

    [Parameter] public string InitialTitle { get; set; }

    private bool CanConfirm => !string.IsNullOrWhiteSpace( name );

    private string Title => title ?? "New Formula Field";

    /// <summary>
    /// Raised when a formula field name is confirmed.
    /// </summary>
    [Parameter] public EventCallback<string> Confirmed { get; set; }

    #endregion

    #region Overrides

    protected override void OnInitialized()
    {
        name = InitialValue;
        title = InitialTitle;
    }

    #endregion
}