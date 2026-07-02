#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Renders a read-only data source selector inside the report designer properties panel.
/// </summary>
public partial class _ReportDesignerDataSourceProperty
{
    #region Methods

    private Task OnClicked()
    {
        return Clicked.InvokeAsync();
    }

    #endregion

    #region Properties

    private string DisplayValue => string.IsNullOrWhiteSpace( DisplayText )
        ? string.IsNullOrWhiteSpace( Value ) ? "None" : Value
        : DisplayText;

    /// <summary>
    /// Property label.
    /// </summary>
    [Parameter] public string Label { get; set; }

    /// <summary>
    /// Selected data source name.
    /// </summary>
    [Parameter] public string Value { get; set; }

    /// <summary>
    /// Friendly text shown for the selected data source.
    /// </summary>
    [Parameter] public string DisplayText { get; set; }

    /// <summary>
    /// Raised when the picker button is clicked.
    /// </summary>
    [Parameter] public EventCallback Clicked { get; set; }

    #endregion
}