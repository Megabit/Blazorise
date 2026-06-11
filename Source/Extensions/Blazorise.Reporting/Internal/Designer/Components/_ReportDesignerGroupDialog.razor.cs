#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Internal dialog used to configure group insertion around a detail band.
/// </summary>
public partial class _ReportDesignerGroupDialog
{
    #region Members

    private readonly List<ReportDesignerFieldOption> fields = [];

    private Modal modalRef;

    private string selectedFieldKey;

    #endregion

    #region Methods

    internal async Task ShowAsync( IEnumerable<ReportDesignerFieldOption> fieldOptions, string selectedFieldName )
    {
        fields.Clear();
        fields.AddRange( fieldOptions ?? [] );

        selectedFieldKey = ResolveInitialFieldKey( selectedFieldName );

        await modalRef.Show();
    }

    private Task CloseAsync()
    {
        return modalRef.Hide();
    }

    private async Task ConfirmAsync()
    {
        var selectedField = FindSelectedField();

        if ( selectedField is null )
            return;

        await Confirmed.InvokeAsync( selectedField.FieldName );
        await modalRef.Hide();
    }

    private static string CreateFieldKey( ReportDesignerFieldOption field )
    {
        return $"{field.DataSourceName}\u001f{field.FieldName}";
    }

    private ReportDesignerFieldOption FindSelectedField()
    {
        return fields.FirstOrDefault( field => string.Equals( CreateFieldKey( field ), selectedFieldKey, StringComparison.Ordinal ) );
    }

    private Task OnSelectedFieldChanged( string value )
    {
        selectedFieldKey = value;

        return Task.CompletedTask;
    }

    private string ResolveInitialFieldKey( string selectedFieldName )
    {
        var selectedField = fields.FirstOrDefault( field => string.Equals( field.FieldName, selectedFieldName, StringComparison.OrdinalIgnoreCase ) )
            ?? fields.FirstOrDefault();

        return selectedField is null ? null : CreateFieldKey( selectedField );
    }

    #endregion

    #region Properties

    private bool CanConfirm => fields.Count > 0 && FindSelectedField() is not null;

    /// <summary>
    /// Raised when the group field selection is confirmed.
    /// </summary>
    [Parameter] public EventCallback<string> Confirmed { get; set; }

    #endregion
}