#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Renders a boolean property editor with a formula editor button.
/// </summary>
public partial class _ReportDesignerFormulaBooleanProperty
{
    #region Properties

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