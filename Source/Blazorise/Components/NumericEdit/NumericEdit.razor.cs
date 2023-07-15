#region Using directives
using System;
using System.Globalization;
using System.Linq.Expressions;
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
/// <typeparam name="TValue">Data-type to be binded by the <see cref="Value"/> property.</typeparam>
public partial class NumericEdit<TValue> : BaseTextInput<TValue>, IAsyncDisposable
{
    #region Members

    /// <summary>
    /// True if the TValue is an integer type.
    /// </summary>
    private bool isIntegerType;

    /// <summary>
    /// Contains the correct inputmode for the input element, based in the TValue.
    /// </summary>
    private string inputMode;

    /// <summary>
    /// Indicates if <see cref="Min"/> parameter is defined.
    /// </summary>
    private bool minDefined = false;

    /// <summary>
    /// Indicates if <see cref="Max"/> parameter is defined.
    /// </summary>
    private bool maxDefined = false;

    #endregion

    #region Constructors

    /// <summary>
    /// Default NumericEdit constructor.
    /// </summary>
    public NumericEdit() : base()
    {
        isIntegerType = TypeHelper.IsInteger( typeof( TValue ) );
        inputMode = isIntegerType ? "numeric" : "decimal";
    }

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

        if ( ParentValidation != null )
        {
            if ( parameters.TryGetValue<Expression<Func<TValue>>>( nameof( ValueExpression ), out var expression ) )
                await ParentValidation.InitializeInputExpression( expression );

            if ( parameters.TryGetValue<string>( nameof( Pattern ), out var pattern ) )
            {
                // make sure we get the newest value
                var value = parameters.TryGetValue<TValue>( nameof( Value ), out var inValue )
                    ? inValue
                    : InternalValue;

                await ParentValidation.InitializeInputPattern( pattern, value );
            }

            await InitializeValidation();
        }
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.NumericEdit( Plaintext ) );
        builder.Append( ClassProvider.NumericEditSize( ThemeSize ) );
        builder.Append( ClassProvider.NumericEditColor( Color ) );
        builder.Append( ClassProvider.NumericEditValidation( ParentValidation?.Status ?? ValidationStatus.None ), ParentValidation?.Status != ValidationStatus.None );

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
        if ( number is IComparable comparableNumber && comparableNumber != null )
        {
            if ( maxDefined && Max is IComparable comparableMax && comparableNumber.CompareTo( comparableMax ) >= 0 )
            {
                comparableNumber = comparableMax;
            }
            else if ( minDefined && Min is IComparable comparableMin && comparableNumber.CompareTo( comparableMin ) <= 0 )
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

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    /// <inheritdoc/>
    protected override TValue InternalValue { get => Value; set => Value = value; }

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
    /// Gets or sets an expression that identifies the value.
    /// </summary>
    [Parameter] public Expression<Func<TValue>> ValueExpression { get; set; }

    /// <summary>
    /// Specifies the interval between valid values.
    /// </summary>
    [Parameter] public decimal? Step { get; set; } = 1;

    /// <summary>
    /// Helps define the language of an element.
    /// </summary>
    /// <remarks>
    /// https://www.w3schools.com/tags/ref_language_codes.asp
    /// </remarks>
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
    /// The size attribute specifies the visible width, in characters, of an input element. https://www.w3schools.com/tags/att_input_size.asp
    /// </summary>
    [Parameter] public int? VisibleCharacters { get; set; }

    #endregion
}