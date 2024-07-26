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
    #region Members

    /// <summary>
    /// Indicates if <see cref="Min"/> parameter is defined.
    /// </summary>
    private bool minDefined = false;

    /// <summary>
    /// Indicates if <see cref="Max"/> parameter is defined.
    /// </summary>
    private bool maxDefined = false;

    #endregion

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

        // This make sure we know that Min or Max parameters are defined and can be checked against the current value.
        // Without we cannot determine if Min or Max has a default value when TValue is non-nullable type.
        minDefined = parameters.TryGetValue<TValue>( nameof( Min ), out var min );
        maxDefined = parameters.TryGetValue<TValue>( nameof( Max ), out var max );

        await base.SetParametersAsync( parameters );

        if ( ParentValidation is not null )
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

    /// <inheritdoc/>
    protected override string GetFormatedValueExpression()
    {
        if ( ValueExpression is null )
            return null;

        return HtmlFieldPrefix is not null
            ? HtmlFieldPrefix.GetFieldName( ValueExpression )
            : ExpressionFormatter.FormatLambda( ValueExpression );
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    /// <inheritdoc/>
    protected override TValue InternalValue { get => Value; set => Value = value; }

    /// <summary>
    /// Gets the string representation of the <see cref="Step"/> value.
    /// </summary>
    protected string StepString => Step.ToCultureInvariantString();

    /// <summary>
    /// Gets the string representation of the <see cref="Min"/> value.
    /// </summary>
    protected string MinString => Min.ToCultureInvariantString();

    /// <summary>
    /// Gets the string representation of the <see cref="Max"/> value.
    /// </summary>
    protected string MaxString => Max.ToCultureInvariantString();

    /// <summary>
    /// Indicates if <see cref="Min"/> parameter is defined.
    /// </summary>
    protected bool MinDefined => minDefined;

    /// <summary>
    /// Indicates if <see cref="Max"/> parameter is defined.
    /// </summary>
    protected bool MaxDefined => maxDefined;

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