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
public partial class Switch<TValue> : BaseCheckComponent<TValue>
{
    #region Members

    private Color color = Color.Default;

    #endregion

    #region Methods

    /// <inheritdoc/>
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        if ( Rendered )
        {
            if ( parameters.TryGetValue<TValue>( nameof( Checked ), out var paramChecked ) && !paramChecked.IsEqual( Checked ) )
            {
                ExecuteAfterRender( async () =>
                {
                    await Revalidate();

                    // Some providers may require that we define classname based on a switch state so we need to reset classes.
                    DirtyClasses();
                    await InvokeAsync( StateHasChanged );
                } );
            }
        }

        await base.SetParametersAsync( parameters );

        if ( ParentValidation is not null )
        {
            if ( parameters.TryGetValue<Expression<Func<TValue>>>( nameof( CheckedExpression ), out var expression ) )
                await ParentValidation.InitializeInputExpression( expression );

            await InitializeValidation();
        }
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.Switch() );
        builder.Append( ClassProvider.SwitchColor( Color ), Color != Color.Default );
        builder.Append( ClassProvider.SwitchSize( ThemeSize ), ThemeSize != Blazorise.Size.Default );
        builder.Append( ClassProvider.SwitchChecked( IsChecked ) );
        builder.Append( ClassProvider.SwitchCursor( Cursor ), Cursor != Cursor.Default );
        builder.Append( ClassProvider.SwitchValidation( ParentValidation?.Status ?? ValidationStatus.None ), ParentValidation?.Status != ValidationStatus.None );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override string TrueValueName => "true";

    /// <summary>
    /// Returns true id switch is in checked state.
    /// </summary>
    protected bool IsChecked => string.Compare( Checked?.ToString(), TrueValueName, StringComparison.InvariantCultureIgnoreCase ) == 0;

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

    #endregion
}