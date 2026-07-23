#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Renders a boolean property editor with a formula editor button.
/// </summary>
public partial class _ReportDesignerFormulaBooleanProperty
{
    #region Methods

    private Task OnValueChanged( bool? value )
        => value.HasValue ? Changed.InvokeAsync( value.Value ) : Task.CompletedTask;

    #endregion

    #region Properties

    private bool? DisplayValue => Mixed ? null : Value;

    private Color FormulaButtonColor => string.IsNullOrWhiteSpace( Formula ) ? Color.Light : Color.Primary;

    /// <summary>
    /// Property label.
    /// </summary>
    [Parameter] public string Label { get; set; }

    /// <summary>
    /// Current fallback value.
    /// </summary>
    [Parameter] public bool Value { get; set; }

    /// <summary>
    /// Indicates that selected elements have different fallback values.
    /// </summary>
    [Parameter] public bool Mixed { get; set; }

    /// <summary>
    /// Current formula expression.
    /// </summary>
    [Parameter] public string Formula { get; set; }

    /// <summary>
    /// Raised when the fallback value changes.
    /// </summary>
    [Parameter] public EventCallback<bool> Changed { get; set; }

    /// <summary>
    /// Opens the formula editor.
    /// </summary>
    [Parameter] public EventCallback OpenFormula { get; set; }

    #endregion
}