#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Gantt.Components;

/// <summary>
/// Internal progress range editor used by Gantt item modal.
/// </summary>
public partial class _GanttProgressEditor : BaseComponent
{
    #region Members

    private bool rendered;
    private bool shouldRender = true;

    #endregion

    #region Methods

    /// <inheritdoc />
    public override Task SetParametersAsync( ParameterView parameters )
    {
        var valueChanged = parameters.TryGetValue<int>( nameof( Value ), out var newValue )
            && newValue != Value;
        var disabledChanged = parameters.TryGetValue<bool>( nameof( Disabled ), out var newDisabled )
            && newDisabled != Disabled;

        shouldRender = !rendered || valueChanged || disabledChanged;

        return base.SetParametersAsync( parameters );
    }

    /// <inheritdoc />
    protected override bool ShouldRender()
        => shouldRender;

    /// <inheritdoc />
    protected override Task OnAfterRenderAsync( bool firstRender )
    {
        rendered = true;
        return base.OnAfterRenderAsync( firstRender );
    }

    private Task OnValueChanged( int value )
        => ValueChanged.InvokeAsync( value );

    #endregion

    #region Properties

    /// <summary>
    /// Specifies progress value in percentage range 0..100.
    /// </summary>
    [Parameter] public int Value { get; set; }

    /// <summary>
    /// Notifies when <see cref="Value"/> changes.
    /// </summary>
    [Parameter] public EventCallback<int> ValueChanged { get; set; }

    /// <summary>
    /// Determines whether progress editor is disabled.
    /// </summary>
    [Parameter] public bool Disabled { get; set; }

    #endregion
}