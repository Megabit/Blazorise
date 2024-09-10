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
/// The editor that allows you to select a color from a dropdown menu.
/// </summary>
public partial class ColorEdit : BaseInputComponent<string>, ISelectableComponent
{
    #region Members

    #endregion

    #region Methods

    /// <inheritdoc/>
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        if ( Rendered )
        {
            if ( parameters.TryGetValue<string>( nameof( Color ), out var paramColor ) && !paramColor.IsEqual( Color ) )
            {
                ExecuteAfterRender( Revalidate );
            }
        }

        await base.SetParametersAsync( parameters );

        if ( ParentValidation is not null )
        {
            if ( parameters.TryGetValue<Expression<Func<string>>>( nameof( ColorExpression ), out var expression ) )
                await ParentValidation.InitializeInputExpression( expression );

            await InitializeValidation();
        }
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.ColorEdit() );
        builder.Append( ClassProvider.ColorEditSize( ThemeSize ) );

        base.BuildClasses( builder );
    }

    /// <summary>
    /// Handles the input onchange event.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected Task OnChangeHandler( ChangeEventArgs eventArgs )
    {
        return CurrentValueHandler( eventArgs?.Value?.ToString() );
    }

    /// <inheritdoc/>
    protected override Task OnInternalValueChanged( string value )
    {
        return ColorChanged.InvokeAsync( value );
    }

    /// <inheritdoc/>
    protected override string FormatValueAsString( string value )
    {
        return value;
    }

    /// <inheritdoc/>
    protected override Task<ParseValue<string>> ParseValueFromStringAsync( string value )
    {
        return Task.FromResult( new ParseValue<string>( true, value, null ) );
    }

    /// <inheritdoc/>
    public virtual Task Select( bool focus = true )
    {
        return JSUtilitiesModule.Select( ElementRef, ElementId, focus ).AsTask();
    }

    /// <inheritdoc/>
    protected override string GetFormatedValueExpression()
    {
        if ( ColorExpression is null )
            return null;

        return HtmlFieldPrefix is not null
            ? HtmlFieldPrefix.GetFieldName( ColorExpression )
            : ExpressionFormatter.FormatLambda( ColorExpression );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the input color value.
    /// </summary>
    [Obsolete( "The 'Color' property is obsolete and will be removed in future versions. Use 'Value' instead." )]
    [Parameter] public string Color { get => Value; set => Value = value; }

    /// <summary>
    /// Occurs when the color has changed.
    /// </summary>
    [Obsolete( "The 'ColorChanged' property is obsolete and will be removed in future versions. Use 'ValueChanged' instead." )]
    [Parameter] public EventCallback<string> ColorChanged { get => ValueChanged; set => ValueChanged = value; }

    /// <summary>
    /// Gets or sets an expression that identifies the color value.
    /// </summary>
    [Obsolete( "The 'ColorExpression' property is obsolete and will be removed in future versions. Use 'ValueExpression' instead." )]
    [Parameter] public Expression<Func<string>> ColorExpression { get => ValueExpression; set => ValueExpression = value; }

    #endregion
}