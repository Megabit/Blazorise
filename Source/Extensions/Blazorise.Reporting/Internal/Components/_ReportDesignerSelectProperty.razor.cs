#region Using directives
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Renders a value-type select editor inside the report designer properties panel.
/// </summary>
/// <typeparam name="TValue">Value type edited by the property.</typeparam>
public partial class _ReportDesignerSelectProperty<TValue>
    where TValue : struct
{
    #region Methods

    private Task OnValueChanged( TValue? value )
        => value.HasValue ? Changed.InvokeAsync( value.Value ) : Task.CompletedTask;

    #endregion

    #region Properties

    private TValue? DisplayValue => Mixed ? null : Value;

    private IReadOnlyList<(TValue Value, string Text)> ResolvedOptions => Options ?? [];

    /// <summary>
    /// Property label.
    /// </summary>
    [Parameter] public string Label { get; set; }

    /// <summary>
    /// Current value.
    /// </summary>
    [Parameter] public TValue Value { get; set; }

    /// <summary>
    /// Indicates that selected elements have different values.
    /// </summary>
    [Parameter] public bool Mixed { get; set; }

    /// <summary>
    /// Available select options.
    /// </summary>
    [Parameter] public IReadOnlyList<(TValue Value, string Text)> Options { get; set; }

    /// <summary>
    /// Raised when the value changes.
    /// </summary>
    [Parameter] public EventCallback<TValue> Changed { get; set; }

    #endregion
}