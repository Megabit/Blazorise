#region Using directives
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Renders a string select property editor with a formula editor button.
/// </summary>
public partial class _ReportDesignerFormulaSelectProperty
{
    #region Properties

    private Color FormulaButtonColor => string.IsNullOrWhiteSpace( Formula ) ? Color.Light : Color.Primary;

    private IReadOnlyList<(string Value, string Text)> ResolvedOptions => Options ?? [];

    /// <summary>
    /// Property label.
    /// </summary>
    [Parameter] public string Label { get; set; }

    /// <summary>
    /// Current fallback value.
    /// </summary>
    [Parameter] public string Value { get; set; }

    /// <summary>
    /// Current formula expression.
    /// </summary>
    [Parameter] public string Formula { get; set; }

    /// <summary>
    /// Available select options.
    /// </summary>
    [Parameter] public IReadOnlyList<(string Value, string Text)> Options { get; set; }

    /// <summary>
    /// Raised when the fallback value changes.
    /// </summary>
    [Parameter] public EventCallback<string> Changed { get; set; }

    /// <summary>
    /// Opens the formula editor.
    /// </summary>
    [Parameter] public EventCallback OpenFormula { get; set; }

    #endregion
}