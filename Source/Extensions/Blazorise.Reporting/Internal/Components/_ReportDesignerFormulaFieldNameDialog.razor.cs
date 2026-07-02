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

    private Modal modalRef;

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
        name = value;
        this.title = title;

        await modalRef.Show();
    }

    private Task CloseAsync()
    {
        return modalRef.Hide();
    }

    private async Task ConfirmAsync()
    {
        if ( !CanConfirm )
            return;

        await Confirmed.InvokeAsync( name.Trim() );
        await modalRef.Hide();
    }

    private Task NameChanged( string value )
    {
        name = value;

        return Task.CompletedTask;
    }

    #endregion

    #region Properties

    private bool CanConfirm => !string.IsNullOrWhiteSpace( name );

    private string Title => title ?? "New Formula Field";

    /// <summary>
    /// Raised when a formula field name is confirmed.
    /// </summary>
    [Parameter] public EventCallback<string> Confirmed { get; set; }

    #endregion
}