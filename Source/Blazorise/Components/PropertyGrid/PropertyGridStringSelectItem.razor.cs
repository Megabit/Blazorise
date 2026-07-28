#region Using directives
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Renders a string select property editor.
/// </summary>
public partial class PropertyGridStringSelectItem : BasePropertyGridEditorItem
{
    #region Members

    private const string MixedValue = "__b_property_grid_mixed__";

    #endregion

    #region Methods

    private Task OnValueChanged( string value )
        => value == MixedValue ? Task.CompletedTask : ValueChanged.InvokeAsync( value );

    #endregion

    #region Properties

    private string DisplayValue => Mixed ? MixedValue : Value;

    private IReadOnlyList<PropertyGridSelectOption<string>> ResolvedOptions => Options ?? [];

    /// <summary>
    /// Gets or sets the property value.
    /// </summary>
    [Parameter] public string Value { get; set; }

    /// <summary>
    /// Occurs when the property value changes.
    /// </summary>
    [Parameter] public EventCallback<string> ValueChanged { get; set; }

    /// <summary>
    /// Defines the available property values.
    /// </summary>
    [Parameter] public IReadOnlyList<PropertyGridSelectOption<string>> Options { get; set; }

    #endregion
}