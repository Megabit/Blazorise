#region Using directives
using System;
using System.Globalization;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise;

/// <summary>
/// An editor that displays a numeric value and allows a user to edit the value.
/// </summary>
/// <typeparam name="TValue">Data-type to be binded by the <see cref="BaseInputComponent{TValue}.Value"/> property.</typeparam>
public partial class NumericInput<TValue> : BaseBufferedTextInput<TValue>, IAsyncDisposable
{
    #region Members

    /// <summary>
    /// True if the TValue is an integer type.
    /// </summary>
    private readonly bool isIntegerType;

    /// <summary>
    /// Contains the correct inputmode for the input element, based in the TValue.
    /// </summary>
    private readonly string inputMode;

    /// <summary>
    /// Captured Min parameter snapshot.
    /// </summary>
    protected ComponentParameterInfo<TValue> paramMin;

    /// <summary>
    /// Captured Max parameter snapshot.
    /// </summary>
    protected ComponentParameterInfo<TValue> paramMax;

    #endregion

    #region Constructors

    /// <summary>
    /// Default NumericInput constructor.
    /// </summary>
    public NumericInput() : base()
    {
        isIntegerType = TypeHelper.IsInteger( typeof( TValue ) );
        inputMode = isIntegerType ? "numeric" : "decimal";
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void CaptureParameters( ParameterView parameters )
    {
        base.CaptureParameters( parameters );

        parameters.TryGetParameter( nameof( Min ), Min, out paramMin );
        parameters.TryGetParameter( nameof( Max ), Max, out paramMax );
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.NumericInput( Plaintext ) );
        builder.Append( ClassProvider.NumericInputSize( ThemeSize ) );
        builder.Append( ClassProvider.NumericInputColor( Color ) );
        builder.Append( ClassProvider.NumericInputValidation( ParentValidation?.Status ?? ValidationStatus.None ) );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override Task<ParseValue<TValue>> ParseValueFromStringAsync( string value )
    {
        if ( Converters.TryChangeType<TValue>( value, out var result, CurrentCultureInfo ) )
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
            byte @byte => Converters.FormatValue( @byte, CurrentCultureInfo ),
            short @short => Converters.FormatValue( @short, CurrentCultureInfo ),
            int @int => Converters.FormatValue( @int, CurrentCultureInfo ),
            long @long => Converters.FormatValue( @long, CurrentCultureInfo ),
            float @float => Converters.FormatValue( @float, CurrentCultureInfo ),
            double @double => Converters.FormatValue( @double, CurrentCultureInfo ),
            decimal @decimal => Converters.FormatValue( @decimal, CurrentCultureInfo ),
            sbyte @sbyte => Converters.FormatValue( @sbyte, CurrentCultureInfo ),
            ushort @ushort => Converters.FormatValue( @ushort, CurrentCultureInfo ),
            uint @uint => Converters.FormatValue( @uint, CurrentCultureInfo ),
            ulong @ulong => Converters.FormatValue( @ulong, CurrentCultureInfo ),
            _ => throw new InvalidOperationException( $"Unsupported type {value.GetType()}" ),
        };
    }

    /// <inheritdoc/>
    protected override async Task OnBlurHandler( FocusEventArgs eventArgs )
    {
        await base.OnBlurHandler( eventArgs );

        if ( !string.IsNullOrEmpty( CurrentValueAsString ) )
        {
            await ProcessNumber( CurrentValue );
        }
    }

    /// <summary>
    /// Process the newly changed number and adjust it if needed.
    /// </summary>
    /// <param name="number">New number value.</param>
    /// <returns>Returns the awaitable task.</returns>
    protected virtual Task ProcessNumber( TValue number )
    {
        if ( number is IComparable comparableNumber && comparableNumber is not null )
        {
            if ( MaxDefined && Max is IComparable comparableMax && comparableNumber.CompareTo( comparableMax ) >= 0 )
            {
                comparableNumber = comparableMax;
            }
            else if ( MinDefined && Min is IComparable comparableMin && comparableNumber.CompareTo( comparableMin ) <= 0 )
            {
                comparableNumber = comparableMin;
            }

            // cast back to TValue and check if number has changed
            if ( Converters.TryChangeType<TValue>( comparableNumber, out var currentValue, CurrentCultureInfo )
                && !CurrentValue.IsEqual( currentValue ) )
            {
                // number has changed so we need to re-set the CurrentValue and re-run any validation
                return CurrentValueHandler( FormatValueAsString( currentValue ) );
            }
        }

        return Task.CompletedTask;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Indicates if <see cref="Min"/> parameter is defined.
    /// </summary>
    private bool MinDefined => paramMin.Defined;

    /// <summary>
    /// Indicates if <see cref="Max"/> parameter is defined.
    /// </summary>
    private bool MaxDefined => paramMax.Defined;

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
    /// Gets the min value if defined, otherwise null.
    /// </summary>
    protected object MinValue => MinDefined ? Min : null;

    /// <summary>
    /// Gets the max value if defined, otherwise null.
    /// </summary>
    protected object MaxValue => MaxDefined ? Max : null;

    /// <summary>
    /// Gets the culture info defined on the input field.
    /// </summary>
    protected CultureInfo CurrentCultureInfo
    {
        get
        {
            if ( !string.IsNullOrEmpty( Culture ) )
            {
                return CultureInfo.GetCultureInfo( Culture );
            }

            return CultureInfo.InvariantCulture;
        }
    }

    /// <summary>
    /// Gets the correct inputmode for the input element, based in the TValue.
    /// </summary>
    protected string InputMode => inputMode;

    /// <summary>
    /// Specifies the interval between valid values.
    /// </summary>
    [Parameter] public decimal? Step { get; set; } = 1;

    /// <summary>
    /// Helps define the language of an element. See <see href="https://www.w3schools.com/tags/ref_language_codes.asp">w3schools.com</see>.
    /// </summary>
    [Parameter] public string Culture { get; set; }

    /// <summary>
    /// The minimum value to accept for this input.
    /// </summary>
    [Parameter] public TValue Min { get; set; }

    /// <summary>
    /// The maximum value to accept for this input.
    /// </summary>
    [Parameter] public TValue Max { get; set; }

    /// <summary>
    /// The size attribute specifies the visible width, in characters, of an input element. See <see href="https://www.w3schools.com/tags/att_input_size.asp">w3schools.com</see>.
    /// </summary>
    [Parameter] public int? VisibleCharacters { get; set; }

    #endregion
}