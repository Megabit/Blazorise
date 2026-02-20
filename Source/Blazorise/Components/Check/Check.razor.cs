#region Using directives
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Checkboxes allow the user to select one or more items from a set.
/// </summary>
/// <typeparam name="TValue">Checked value type.</typeparam>
public partial class Check<TValue> : BaseCheckComponent<TValue, CheckClasses, CheckStyles>
{
    #region Members

    private bool? indeterminate;

    /// <summary>
    /// Captured Indeterminate parameter snapshot.
    /// </summary>
    protected ComponentParameterInfo<bool?> paramIndeterminate;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void CaptureParameters( ParameterView parameters )
    {
        base.CaptureParameters( parameters );

        parameters.TryGetParameter( Indeterminate, out paramIndeterminate );
    }

    /// <inheritdoc/>
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        await base.SetParametersAsync( parameters );

        if ( paramIndeterminate.Defined && paramIndeterminate.Changed )
        {
            indeterminate = paramIndeterminate.Value;

            ExecuteAfterRender( async () =>
            {
                await JSUtilitiesModule.SetProperty( ElementRef, "indeterminate", indeterminate );
            } );
        }
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.Check() );
        builder.Append( ClassProvider.CheckSize( ThemeSize ) );
        builder.Append( ClassProvider.CheckCursor( Cursor ) );
        builder.Append( ClassProvider.CheckValidation( ParentValidation?.Status ?? ValidationStatus.None ) );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the string representation of a boolean "true" value.
    /// </summary>
    protected override string TrueValueName => "true";

    /// <summary>
    /// The indeterminate property can help you to achieve a 'check all' effect.
    /// </summary>
    [Parameter] public bool? Indeterminate { get; set; }

    /// <summary>
    /// Defines the name attribute of a checkbox.
    /// </summary>
    /// <remarks>
    /// The name attribute is used to identify form data after it has been submitted to the server, or to reference form data using JavaScript on the client side.
    /// </remarks>
    [Parameter] public string Name { get; set; }

    #endregion
}