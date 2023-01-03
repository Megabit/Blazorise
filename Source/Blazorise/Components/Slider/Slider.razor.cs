#region Using directives
using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// A slider to select a value from a given range.
/// </summary>
/// <typeparam name="TValue">Data-type to be binded by the <see cref="Value"/> property.</typeparam>
public partial class Slider<TValue> : BaseInputComponent<TValue>
{
    #region Methods

    /// <inheritdoc/>
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        if ( Rendered )
        {
            if ( parameters.TryGetValue<TValue>( nameof( Value ), out var paramValue ) && !paramValue.IsEqual( Value ) )
            {
                ExecuteAfterRender( Revalidate );
            }
        }

        await base.SetParametersAsync( parameters );

        if ( ParentValidation != null )
        {
            if ( parameters.TryGetValue<Expression<Func<TValue>>>( nameof( ValueExpression ), out var expression ) )
                await ParentValidation.InitializeInputExpression( expression );

            await InitializeValidation();
        }
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.Slider() );
        builder.Append( ClassProvider.SliderValidation( ParentValidation?.Status ?? ValidationStatus.None ), ParentValidation?.Status != ValidationStatus.None );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override Task OnInternalValueChanged( TValue value )
    {
        return ValueChanged.InvokeAsync( value );
    }

    /// <inheritdoc/>
    protected override Task<ParseValue<TValue>> ParseValueFromStringAsync( string value )
    {
        if ( Converters.TryChangeType<TValue>( value, out var result, CultureInfo.InvariantCulture ) )
        {
            return Task.FromResult( new ParseValue<TValue>( true, result, null ) );
        }
        else
        {
            return Task.FromResult( ParseValue<TValue>.Empty );
        }
    }

    /// <inheritdoc/>
    protected override string FormatValueAsString( TValue value )
    {
        return value switch
        {
            null => null,
            byte @byte => Converters.FormatValue( @byte, CultureInfo.InvariantCulture ),
            short @short => Converters.FormatValue( @short, CultureInfo.InvariantCulture ),
            int @int => Converters.FormatValue( @int, CultureInfo.InvariantCulture ),
            long @long => Converters.FormatValue( @long, CultureInfo.InvariantCulture ),
            float @float => Converters.FormatValue( @float, CultureInfo.InvariantCulture ),
            double @double => Converters.FormatValue( @double, CultureInfo.InvariantCulture ),
            decimal @decimal => Converters.FormatValue( @decimal, CultureInfo.InvariantCulture ),
            sbyte @sbyte => Converters.FormatValue( @sbyte, CultureInfo.InvariantCulture ),
            ushort @ushort => Converters.FormatValue( @ushort, CultureInfo.InvariantCulture ),
            uint @uint => Converters.FormatValue( @uint, CultureInfo.InvariantCulture ),
            ulong @ulong => Converters.FormatValue( @ulong, CultureInfo.InvariantCulture ),
            _ => throw new InvalidOperationException( $"Unsupported type {value.GetType()}" ),
        };
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    /// <inheritdoc/>
    protected override TValue InternalValue { get => Value; set => Value = value; }

    /// <summary>
    /// Specifies the interval between valid values.
    /// </summary>
    [Parameter] public TValue Step { get; set; }

    /// <summary>
    /// Gets or sets the value inside the input field.
    /// </summary>
    [Parameter] public TValue Value { get; set; }

    /// <summary>
    /// Occurs after the value has changed.
    /// </summary>
    /// <remarks>
    /// This will be converted to EventCallback once the Blazor team fix the error for generic components. see https://github.com/aspnet/AspNetCore/issues/8385
    /// </remarks>
    [Parameter] public EventCallback<TValue> ValueChanged { get; set; }

    /// <summary>
    /// Gets or sets an expression that identifies the checked value.
    /// </summary>
    [Parameter] public Expression<Func<TValue>> ValueExpression { get; set; }

    /// <summary>
    /// The minimum value to accept for this input.
    /// </summary>
    [Parameter] public TValue Min { get; set; }

    /// <summary>
    /// The maximum value to accept for this input.
    /// </summary>
    [Parameter] public TValue Max { get; set; }

    #endregion
}