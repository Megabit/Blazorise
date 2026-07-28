#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Renders a text property editor.
/// </summary>
public partial class PropertyGridTextItem : BasePropertyGridEditorItem
{
    private string DisplayValue => Mixed ? null : Value;

    private bool EffectiveImmediate => Immediate ?? ValueChanged.HasDelegate;

    /// <summary>
    /// Gets or sets the property value.
    /// </summary>
    [Parameter] public string Value { get; set; }

    /// <summary>
    /// Occurs when the property value changes.
    /// </summary>
    [Parameter] public EventCallback<string> ValueChanged { get; set; }

    /// <summary>
    /// Defines whether the property is read-only.
    /// </summary>
    [Parameter] public bool ReadOnly { get; set; }

    /// <summary>
    /// Defines whether value changes are reported immediately.
    /// </summary>
    [Parameter] public bool? Immediate { get; set; }
}
