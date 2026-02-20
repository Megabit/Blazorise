#region Using directives
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Switches toggle the state of a single setting on or off.
/// </summary>
/// <typeparam name="TValue">Checked value type.</typeparam>
public partial class Switch<TValue> : BaseCheckComponent<TValue, SwitchClasses, SwitchStyles>
{
    #region Members

    private Color color = Color.Default;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override async Task OnBeforeSetParametersAsync( ParameterView parameters )
    {
        await base.OnBeforeSetParametersAsync( parameters );

        if ( Rendered )
        {
            if ( paramValue.Changed )
            {
                ExecuteAfterRender( async () =>
                {
                    // Some providers may require that we define classname based on a switch state so we need to reset classes.
                    DirtyClasses();
                    await InvokeAsync( StateHasChanged );
                } );
            }
        }
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.Switch() );
        builder.Append( ClassProvider.SwitchColor( Color ) );
        builder.Append( ClassProvider.SwitchSize( ThemeSize ) );
        builder.Append( ClassProvider.SwitchChecked( IsChecked ) );
        builder.Append( ClassProvider.SwitchCursor( Cursor ) );
        builder.Append( ClassProvider.SwitchValidation( ParentValidation?.Status ?? ValidationStatus.None ) );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override string TrueValueName => "true";

    /// <summary>
    /// Returns true id switch is in checked state.
    /// </summary>
    protected bool IsChecked => string.Compare( Value?.ToString(), TrueValueName, StringComparison.InvariantCultureIgnoreCase ) == 0;

    /// <summary>
    /// Defines the switch named color.
    /// </summary>
    [Parameter]
    public Color Color
    {
        get { return color; }
        set
        {
            color = value;
            DirtyClasses();
        }
    }

    /// <summary>
    /// Defines the switch named intent.
    /// </summary>
    [Parameter]
    public Intent Intent
    {
        get => Color.ToIntent();
        set => Color = value.ToColor();
    }

    #endregion
}