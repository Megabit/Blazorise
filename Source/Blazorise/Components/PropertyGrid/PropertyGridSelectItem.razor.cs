#region Using directives
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Renders a value-type select property editor.
/// </summary>
/// <typeparam name="TValue">The option value type.</typeparam>
public partial class PropertyGridSelectItem<TValue> : BasePropertyGridEditorItem
    where TValue : struct
{
    private TValue? DisplayValue => Mixed ? null : Value;

    private IReadOnlyList<PropertyGridSelectOption<TValue>> ResolvedOptions => Options ?? [];

    private Task OnValueChanged( TValue? value )
        => value.HasValue ? ValueChanged.InvokeAsync( value.Value ) : Task.CompletedTask;

    /// <summary>
    /// Gets or sets the property value.
    /// </summary>
    [Parameter] public TValue Value { get; set; }

    /// <summary>
    /// Occurs when the property value changes.
    /// </summary>
    [Parameter] public EventCallback<TValue> ValueChanged { get; set; }

    /// <summary>
    /// Defines the available property values.
    /// </summary>
    [Parameter] public IReadOnlyList<PropertyGridSelectOption<TValue>> Options { get; set; }
}
