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

    internal async Task ShowAsync( string value )
    {
        await ShowAsync( value, null );
    }

    internal async Task ShowAsync( string value, string title )
    {
        await ShowReportModalAsync<_ReportDesignerFormulaFieldNameDialog>( parameters =>
        {
            parameters.Add( nameof( InitialValue ), value );
            parameters.Add( nameof( InitialTitle ), title );
            parameters.Add( nameof( Confirmed ), Confirmed );
        } );
    }

    private Task CloseAsync()
    {
        return CloseReportModalAsync();
    }

    private async Task ConfirmAsync()
    {
        if ( !CanConfirm )
            return;

        await Confirmed.InvokeAsync( name.Trim() );
        await CloseReportModalAsync();
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