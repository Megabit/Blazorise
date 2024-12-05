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
/// <typeparam name="TValue">Data-type to be binded by the <see cref="BaseInputComponent{TValue}.Value"/> property.</typeparam>
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
    protected override async Task OnBeforeSetParametersAsync( ParameterView parameters )
    {
        await base.OnBeforeSetParametersAsync( parameters );

        // This make sure we know that Min or Max parameters are defined and can be checked against the current value.
        // Without we cannot determine if Min or Max has a default value when TValue is non-nullable type.
        minDefined = parameters.TryGetValue<TValue>( nameof( Min ), out var min );
        maxDefined = parameters.TryGetValue<TValue>( nameof( Max ), out var max );
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.Slider() );
        builder.Append( ClassProvider.SliderValidation( ParentValidation?.Status ?? ValidationStatus.None ) );

        base.BuildClasses( builder );
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
    /// The minimum value to accept for this input.
    /// </summary>
    [Parameter] public TValue Min { get; set; }

    /// <summary>
    /// The maximum value to accept for this input.
    /// </summary>
    [Parameter] public TValue Max { get; set; }

    #endregion
}