#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Renders a boolean property editor.
/// </summary>
public partial class PropertyGridBooleanItem : BasePropertyGridEditorItem
{
    #region Methods

    private Task OnValueChanged( bool? value )
        => value.HasValue ? ValueChanged.InvokeAsync( value.Value ) : Task.CompletedTask;

    #endregion

    #region Properties

    private bool? DisplayValue => Mixed ? null : Value;

    /// <summary>
    /// Gets or sets the property value.
    /// </summary>
    [Parameter] public bool Value { get; set; }

    /// <summary>
    /// Occurs when the property value changes.
    /// </summary>
    [Parameter] public EventCallback<bool> ValueChanged { get; set; }

    /// <summary>
    /// Defines the text displayed for a true value.
    /// </summary>
    [Parameter] public string TrueText { get; set; } = "True";

    /// <summary>
    /// Defines the text displayed for a false value.
    /// </summary>
    [Parameter] public string FalseText { get; set; } = "False";

    #endregion
}