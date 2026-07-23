#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Renders a boolean editor inside the report designer properties panel.
/// </summary>
public partial class _ReportDesignerCheckboxProperty
{
    #region Methods

    private Task OnValueChanged( bool? value )
        => value.HasValue ? Changed.InvokeAsync( value.Value ) : Task.CompletedTask;

    #endregion

    #region Properties

    private bool? DisplayValue => Mixed ? null : Value;

    /// <summary>
    /// Property label.
    /// </summary>
    [Parameter] public string Label { get; set; }

    /// <summary>
    /// Current boolean value.
    /// </summary>
    [Parameter] public bool Value { get; set; }

    /// <summary>
    /// Indicates that selected elements have different values.
    /// </summary>
    [Parameter] public bool Mixed { get; set; }

    /// <summary>
    /// Raised when the checkbox value changes.
    /// </summary>
    [Parameter] public EventCallback<bool> Changed { get; set; }

    #endregion
}