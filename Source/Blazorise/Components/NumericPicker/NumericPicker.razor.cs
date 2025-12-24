#region Using directives
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Modules;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
#endregion

namespace Blazorise;

/// <summary>
/// An editor that displays a numeric value and allows a user to edit the value.
/// </summary>
/// <typeparam name="TValue">Data-type to be binded by the <see cref="BaseInputComponent{TValue}.Value"/> property.</typeparam>
public partial class NumericPicker<TValue> : BaseBufferedTextInput<TValue>, INumericPicker, IAsyncDisposable
{
    #region Members

    /// <summary>
    /// Object reference that can be accessed through the JSInterop.
    /// </summary>
    private DotNetObjectReference<NumericPickerAdapter> dotNetObjectRef;

    /// <summary>
    /// Captured Decimals parameter snapshot.
    /// </summary>
    private ComponentParameterInfo<int> paramDecimals;

    /// <summary>
    /// Captured DecimalSeparator parameter snapshot.
    /// </summary>
    private ComponentParameterInfo<string> paramDecimalSeparator;

    /// <summary>
    /// Captured AlternativeDecimalSeparator parameter snapshot.
    /// </summary>
    private ComponentParameterInfo<string> paramAlternativeDecimalSeparator;

    /// <summary>
    /// Captured GroupSeparator parameter snapshot.
    /// </summary>
    private ComponentParameterInfo<string> paramGroupSeparator;

    /// <summary>
    /// Captured GroupSpacing parameter snapshot.
    /// </summary>
    private ComponentParameterInfo<string> paramGroupSpacing;

    /// <summary>
    /// Captured CurrencySymbol parameter snapshot.
    /// </summary>
    private ComponentParameterInfo<string> paramCurrencySymbol;

    /// <summary>
    /// Captured CurrencySymbolPlacement parameter snapshot.
    /// </summary>
    private ComponentParameterInfo<CurrencySymbolPlacement> paramCurrencySymbolPlacement;

    /// <summary>
    /// Captured RoundingMethod parameter snapshot.
    /// </summary>
    private ComponentParameterInfo<NumericRoundingMethod> paramRoundingMethod;

    /// <summary>
    /// Captured Min parameter snapshot.
    /// </summary>
    private ComponentParameterInfo<TValue> paramMin;

    /// <summary>
    /// Captured Max parameter snapshot.
    /// </summary>
    private ComponentParameterInfo<TValue> paramMax;

    /// <summary>
    /// Captured MinMaxLimitsOverride parameter snapshot.
    /// </summary>
    private ComponentParameterInfo<NumericMinMaxLimitsOverride> paramMinMaxLimitsOverride;

    /// <summary>
    /// Captured SelectAllOnFocus parameter snapshot.
    /// </summary>
    private ComponentParameterInfo<bool> paramSelectAllOnFocus;

    /// <summary>
    /// Captured AllowDecimalPadding parameter snapshot.
    /// </summary>
    private ComponentParameterInfo<NumericAllowDecimalPadding> paramAllowDecimalPadding;

    /// <summary>
    /// Captured AlwaysAllowDecimalSeparator parameter snapshot.
    /// </summary>
    private ComponentParameterInfo<bool> paramAlwaysAllowDecimalSeparator;

    /// <summary>
    /// Captured ModifyValueOnWheel parameter snapshot.
    /// </summary>
    private ComponentParameterInfo<bool> paramModifyValueOnWheel;

    /// <summary>
    /// Captured WheelOn parameter snapshot.
    /// </summary>
    private ComponentParameterInfo<NumericWheelOn> paramWheelOn;

    /// <summary>
    /// Saves the last received value from the JS. 
    /// </summary>
    private string valueToChangeOnBlur;

    /// <summary>
    /// True if we have received the value from JS.
    /// </summary>
    private bool hasValueToChangeOnBlur;

    /// <summary>
    /// True if the TValue is an integer type.
    /// </summary>
    private bool isIntegerType;

    /// <summary>
    /// Contains the correct inputmode for the input element, based in the TValue.
    /// </summary>
    private string inputMode;

    #endregion

    #region Constructors

    /// <summary>
    /// Default NumericPicker constructor.
    /// </summary>
    public NumericPicker()
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

        parameters.TryGetParameter( nameof( Decimals ), Decimals, out paramDecimals );
        parameters.TryGetParameter( nameof( DecimalSeparator ), DecimalSeparator, out paramDecimalSeparator );
        parameters.TryGetParameter( nameof( AlternativeDecimalSeparator ), AlternativeDecimalSeparator, out paramAlternativeDecimalSeparator );
        parameters.TryGetParameter( nameof( GroupSeparator ), GroupSeparator, out paramGroupSeparator );
        parameters.TryGetParameter( nameof( GroupSpacing ), GroupSpacing, out paramGroupSpacing );
        parameters.TryGetParameter( nameof( CurrencySymbol ), CurrencySymbol, out paramCurrencySymbol );
        parameters.TryGetParameter( nameof( CurrencySymbolPlacement ), CurrencySymbolPlacement, out paramCurrencySymbolPlacement );
        parameters.TryGetParameter( nameof( RoundingMethod ), RoundingMethod, out paramRoundingMethod );
        parameters.TryGetParameter( nameof( Min ), Min, out paramMin );
        parameters.TryGetParameter( nameof( Max ), Max, out paramMax );
        parameters.TryGetParameter( nameof( MinMaxLimitsOverride ), MinMaxLimitsOverride, out paramMinMaxLimitsOverride );
        parameters.TryGetParameter( nameof( SelectAllOnFocus ), SelectAllOnFocus, out paramSelectAllOnFocus );
        parameters.TryGetParameter( nameof( AllowDecimalPadding ), AllowDecimalPadding, out paramAllowDecimalPadding );
        parameters.TryGetParameter( nameof( AlwaysAllowDecimalSeparator ), AlwaysAllowDecimalSeparator, out paramAlwaysAllowDecimalSeparator );
        parameters.TryGetParameter( nameof( ModifyValueOnWheel ), ModifyValueOnWheel, out paramModifyValueOnWheel );
        parameters.TryGetParameter( nameof( WheelOn ), WheelOn, out paramWheelOn );
    }

    /// <inheritdoc/>
    protected override async Task OnBeforeSetParametersAsync( ParameterView parameters )
    {
        await base.OnBeforeSetParametersAsync( parameters );

        if ( Rendered )
        {
            var decimalsChanged = !isIntegerType && paramDecimals.Defined && paramDecimals.Changed;
            var decimalSeparatorChanged = paramDecimalSeparator.Defined && paramDecimalSeparator.Changed;
            var alternativeDecimalSeparatorChanged = paramAlternativeDecimalSeparator.Defined && paramAlternativeDecimalSeparator.Changed;

            var groupSeparatorChanged = paramGroupSeparator.Defined && paramGroupSeparator.Changed;
            var groupSpacingChanged = paramGroupSpacing.Defined && paramGroupSpacing.Changed;

            var currencySymbolChanged = paramCurrencySymbol.Defined && paramCurrencySymbol.Changed;
            var currencySymbolPlacementChanged = paramCurrencySymbolPlacement.Defined && paramCurrencySymbolPlacement.Changed;

            var roundingMethodChanged = paramRoundingMethod.Defined && paramRoundingMethod.Changed;

            var minChanged = paramMin.Defined && paramMin.Changed;
            var maxChanged = paramMax.Defined && paramMax.Changed;
            var minMaxLimitsOverrideChanged = paramMinMaxLimitsOverride.Defined && paramMinMaxLimitsOverride.Changed;

            var selectAllOnFocusChanged = paramSelectAllOnFocus.Defined && paramSelectAllOnFocus.Changed;

            var allowDecimalPaddingChanged = paramAllowDecimalPadding.Defined && paramAllowDecimalPadding.Changed;
            var alwaysAllowDecimalSeparatorChanged = paramAlwaysAllowDecimalSeparator.Defined && paramAlwaysAllowDecimalSeparator.Changed;

            var modifyValueOnWheelChanged = paramModifyValueOnWheel.Defined && paramModifyValueOnWheel.Changed;
            var wheelOnChanged = paramWheelOn.Defined && paramWheelOn.Changed;

            if ( decimalsChanged || decimalSeparatorChanged || alternativeDecimalSeparatorChanged
                || groupSeparatorChanged || groupSpacingChanged
                || currencySymbolChanged || currencySymbolPlacementChanged
                || roundingMethodChanged
                || minChanged || maxChanged
                || selectAllOnFocusChanged
                || allowDecimalPaddingChanged || alwaysAllowDecimalSeparatorChanged
                || modifyValueOnWheelChanged )
            {
                ExecuteAfterRender( async () => await JSModule.UpdateOptions( ElementRef, ElementId, new NumericPickerUpdateJSOptions
                {
                    Decimals = new JSOptionChange<int>( decimalsChanged, GetDecimals() ),
                    DecimalSeparator = new JSOptionChange<string>( decimalSeparatorChanged, paramDecimalSeparator.Value ),
                    AlternativeDecimalSeparator = new JSOptionChange<string>( alternativeDecimalSeparatorChanged, paramAlternativeDecimalSeparator.Value ),
                    GroupSeparator = new JSOptionChange<string>( groupSeparatorChanged, paramGroupSeparator.Value ),
                    GroupSpacing = new JSOptionChange<string>( groupSpacingChanged, paramGroupSpacing.Value ),
                    CurrencySymbol = new JSOptionChange<string>( currencySymbolChanged, paramCurrencySymbol.Value ),
                    CurrencySymbolPlacement = new JSOptionChange<string>( currencySymbolPlacementChanged, paramCurrencySymbolPlacement.Value.ToCurrencySymbolPlacement() ),
                    RoundingMethod = new JSOptionChange<string>( roundingMethodChanged, paramRoundingMethod.Value.ToNumericRoundingMethod() ),
                    AllowDecimalPadding = new JSOptionChange<object>( allowDecimalPaddingChanged, paramAllowDecimalPadding.Value.ToNumericDecimalPadding() ),
                    AlwaysAllowDecimalSeparator = new JSOptionChange<bool>( alwaysAllowDecimalSeparatorChanged, paramAlwaysAllowDecimalSeparator.Value ),
                    Min = new JSOptionChange<object>( minChanged, paramMin.Value ),
                    Max = new JSOptionChange<object>( maxChanged, paramMax.Value ),
                    MinMaxLimitsOverride = new JSOptionChange<object>( minMaxLimitsOverrideChanged, paramMinMaxLimitsOverride.Value ),
                    SelectAllOnFocus = new JSOptionChange<bool>( selectAllOnFocusChanged, paramSelectAllOnFocus.Value ),
                    ModifyValueOnWheel = new JSOptionChange<bool>( modifyValueOnWheelChanged, paramModifyValueOnWheel.Value ),
                    WheelOn = new JSOptionChange<object>( wheelOnChanged, paramWheelOn.Value.ToNumericWheelOn() ),
                } ) );

            }
        }

        if ( paramValue.Changed )
        {
            ExecuteAfterRender( async () => await JSModule.UpdateValue( ElementRef, ElementId, paramValue ) );
        }
    }

    /// <inheritdoc/>
    protected override async Task OnFirstAfterRenderAsync()
    {
        dotNetObjectRef ??= CreateDotNetObjectRef( new NumericPickerAdapter( this ) );

        // find the min and max possible value based on the supplied value type
        var (minFromType, maxFromType) = Converters.GetMinMaxValueOfType<TValue>();

        await JSModule.Initialize( dotNetObjectRef, ElementRef, ElementId, new()
        {
            Value = Value,
            Immediate = IsImmediate,
            Debounce = IsDebounce,
            DebounceInterval = DebounceIntervalValue,
            Decimals = GetDecimals(),
            DecimalSeparator = DecimalSeparator,
            AlternativeDecimalSeparator = AlternativeDecimalSeparator,
            GroupSeparator = GroupSeparator,
            GroupSpacing = GroupSpacing,
            CurrencySymbol = CurrencySymbol,
            CurrencySymbolPlacement = CurrencySymbolPlacement.ToCurrencySymbolPlacement(),
            RoundingMethod = RoundingMethod.ToNumericRoundingMethod(),
            AllowDecimalPadding = AllowDecimalPadding.ToNumericDecimalPadding(),
            AlwaysAllowDecimalSeparator = AlwaysAllowDecimalSeparator,
            Min = MinDefined ? (object)Min : null,
            Max = MaxDefined ? (object)Max : null,
            MinMaxLimitsOverride = MinMaxLimitsOverride.ToNumericMinMaxLimitsOverride(),
            TypeMin = minFromType,
            TypeMax = maxFromType,
            Step = Step,
            SelectAllOnFocus = SelectAllOnFocus,
            ModifyValueOnWheel = ModifyValueOnWheel,
            WheelOn = WheelOn.ToNumericWheelOn(),
        } );

        await base.OnFirstAfterRenderAsync();
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing && Rendered )
        {
            await JSModule.SafeDestroy( ElementRef, ElementId );

            DisposeDotNetObjectRef( dotNetObjectRef );
            dotNetObjectRef = null;
        }

        await base.DisposeAsync( disposing );
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.NumericPicker( Plaintext ) );
        builder.Append( ClassProvider.NumericPickerSize( ThemeSize ) );
        builder.Append( ClassProvider.NumericPickerColor( Color ) );
        builder.Append( ClassProvider.NumericPickerValidation( ParentValidation?.Status ?? ValidationStatus.None ) );

        base.BuildClasses( builder );
    }

    /// <summary>
    /// Executes given action after the rendering is done.
    /// </summary>
    /// <remarks>Don't await this on the UI thread, because that will cause a deadlock.</remarks>
    private async Task<T> ExecuteAfterRenderAsync<T>( Func<Task<T>> action, CancellationToken token = default )
    {
        var source = new TaskCompletionSource<T>();

        token.Register( () => source.TrySetCanceled() );

        ExecuteAfterRender( async () =>
        {
            try
            {
                var result = await action();
                source.TrySetResult( result );
            }
            catch ( TaskCanceledException )
            {
                source.TrySetCanceled();
            }
            catch ( Exception e )
            {
                source.TrySetException( e );
            }
        } );

        return await source.Task.ConfigureAwait( false );
    }

    /// <inheritdoc/>
    public async Task SetValue( string value )
    {
        if ( IsImmediate )
        {
            if ( IsDebounce )
            {
                InputValueDebouncer?.Update( value );
            }
            else
                await CurrentValueHandler( value );
        }
        else
        {
            valueToChangeOnBlur = value;
            hasValueToChangeOnBlur = true;
        }
    }

    /// <inheritdoc/>
    protected override Task<ParseValue<TValue>> ParseValueFromStringAsync( string value )
    {
        if ( Converters.TryChangeType<TValue>( value, out var result, CurrentCultureInfo ) )
        {
            return Task.FromResult( new ParseValue<TValue>( true, result, null ) );
        }
        else if ( Converters.TryParseAndLimitLargeNumber<TValue>( value, out var result2, CurrentCultureInfo ) )
        {
            return Task.FromResult( new ParseValue<TValue>( true, result2, null ) );
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
            float @float => Converters.FormatValue( @float, CurrentCultureInfo, GetDecimals() ),
            double @double => Converters.FormatValue( @double, CurrentCultureInfo, GetDecimals() ),
            decimal @decimal => Converters.FormatValue( @decimal, CurrentCultureInfo, GetDecimals() ),
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

        if ( !IsImmediate && hasValueToChangeOnBlur )
        {
            hasValueToChangeOnBlur = false;

            await CurrentValueHandler( valueToChangeOnBlur );
        }

        if ( !string.IsNullOrEmpty( CurrentValueAsString ) )
        {
            await ProcessNumber( CurrentValue );
        }
    }

    /// <inheritdoc/>
    protected override async Task OnKeyDownHandler( KeyboardEventArgs eventArgs )
    {
        await base.OnKeyDownHandler( eventArgs );

        if ( !IsImmediate && hasValueToChangeOnBlur && ( eventArgs.Code == "ArrowUp" || eventArgs.Code == "ArrowDown" ) )
        {
            hasValueToChangeOnBlur = false;

            await CurrentValueHandler( valueToChangeOnBlur );
        }
    }

    /// <inheritdoc/>
    protected override async Task OnKeyPressHandler( KeyboardEventArgs eventArgs )
    {
        await base.OnKeyPressHandler( eventArgs );

        if ( !IsImmediate && hasValueToChangeOnBlur && ( eventArgs?.Key?.Equals( "Enter", StringComparison.OrdinalIgnoreCase ) ?? false ) )
        {
            hasValueToChangeOnBlur = false;

            await CurrentValueHandler( valueToChangeOnBlur );
        }
    }

    /// <summary>
    /// Handles the spin-up button click event.
    /// </summary>
    /// <returns>Returns the awaitable task.</returns>
    protected virtual Task OnSpinUpClicked()
    {
        if ( !IsEnableStep || ReadOnly || Disabled )
            return Task.CompletedTask;

        return ProcessNumber( AddStep( CurrentValue, 1 ) );
    }

    /// <summary>
    /// Handles the spin-down button click event.
    /// </summary>
    /// <returns>Returns the awaitable task.</returns>
    protected virtual Task OnSpinDownClicked()
    {
        if ( !IsEnableStep || ReadOnly || Disabled )
            return Task.CompletedTask;

        return ProcessNumber( AddStep( CurrentValue, -1 ) );
    }

    /// <summary>
    /// Applies the step to the supplied value and returns the result.
    /// </summary>
    /// <param name="value">Value to which we apply the step.</param>
    /// <param name="sign">Defines the positive or negative step direction.</param>
    /// <returns>Returns the new value.</returns>
    protected virtual TValue AddStep( TValue value, int sign )
    {
        // make sure that null values also starts from zero
        value ??= Converters.ChangeType<TValue>( 0 );

        return sign > 0
            ? MathUtils<TValue>.Add( value, Converters.ChangeType<TValue>( Step.GetValueOrDefault( 1 ) ) )
            : MathUtils<TValue>.Subtract( value, Converters.ChangeType<TValue>( Step.GetValueOrDefault( 1 ) ) );
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
                ExecuteAfterRender( async () => await JSModule.UpdateValue( ElementRef, ElementId, currentValue ) );

                // number has changed so we need to re-set the CurrentValue and re-run any validation
                return CurrentValueHandler( FormatValueAsString( currentValue ) );
            }
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Returns the numbers of allowed decimals.
    /// </summary>
    /// <returns>Number of decimals.</returns>
    protected int GetDecimals() => isIntegerType ? 0 : Decimals;

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
    /// True if spin buttons can be shown.
    /// </summary>
    protected bool IsShowStepButtons
        => ShowStepButtons.GetValueOrDefault( Options?.ShowNumericStepButtons ?? true ) && IsEnableStep;

    /// <summary>
    /// True if value can be changed with stepper.
    /// </summary>
    protected bool IsEnableStep
        => EnableStep.GetValueOrDefault( Options?.EnableNumericStep ?? true );

    /// <summary>
    /// Gets the string representation of the <see cref="Step"/> value.
    /// </summary>
    protected string StepString => Step.ToCultureInvariantString();

    /// <summary>
    /// Gets the culture info defined on the input field.
    /// </summary>
    protected CultureInfo CurrentCultureInfo
    {
        get
        {
            // TODO: find the right culture based on DecimalSeparator
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
    /// Gets or sets the <see cref="IJSNumericPickerModule"/> instance.
    /// </summary>
    [Inject] public IJSNumericPickerModule JSModule { get; set; }

    /// <summary>
    /// Specifies the interval between valid values.
    /// </summary>
    [Parameter] public decimal? Step { get; set; } = 1;

    /// <summary>
    /// Maximum number of decimal places after the decimal separator.
    /// </summary>
    [Parameter] public int Decimals { get; set; } = 2;

    /// <summary>
    /// String to use as the decimal separator in numeric values.
    /// </summary>
    [Parameter] public string DecimalSeparator { get; set; } = ".";

    /// <summary>
    /// String to use as the alternative decimal separator in numeric values.
    /// </summary>
    [Parameter] public string AlternativeDecimalSeparator { get; set; } = ",";

    /// <summary>
    /// Defines the thousand grouping separator character.
    /// </summary>
    [Parameter] public string GroupSeparator { get; set; }

    /// <summary>
    /// Defines how many numbers should be grouped together (usually for the thousand separator).
    /// </summary>
    [Parameter] public string GroupSpacing { get; set; } = "3";

    /// <summary>
    /// Defines the currency symbol to display.
    /// </summary>
    [Parameter] public string CurrencySymbol { get; set; }

    /// <summary>
    /// Placement of the currency sign, relative to the number shown (as a prefix or a suffix).
    /// </summary>
    [Parameter] public CurrencySymbolPlacement CurrencySymbolPlacement { get; set; } = CurrencySymbolPlacement.Prefix;

    /// <summary>
    /// Method used for rounding decimal values.
    /// </summary>
    [Parameter] public NumericRoundingMethod RoundingMethod { get; set; } = NumericRoundingMethod.HalfUpSymmetric;

    /// <summary>
    /// Allow padding the decimal places with zeros. If set to <c>Floats</c>, padding is only done when there are some decimals. 
    /// </summary>
    /// <remarks>
    /// Setting AllowDecimalPadding to 'false' will override the <see cref="Decimals"/> setting.
    /// </remarks>
    [Parameter] public NumericAllowDecimalPadding AllowDecimalPadding { get; set; } = NumericAllowDecimalPadding.Always;

    /// <summary>
    /// Defines if the decimal character or decimal character alternative should be accepted when there is already a decimal character shown in the element.
    /// </summary>
    [Parameter] public bool AlwaysAllowDecimalSeparator { get; set; }

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
    /// Override the minimum and maximum limits.
    /// </summary>
    [Parameter] public NumericMinMaxLimitsOverride MinMaxLimitsOverride { get; set; } = NumericMinMaxLimitsOverride.Ignore;

    /// <summary>
    /// The size attribute specifies the visible width, in characters, of an input element. See <see href="https://www.w3schools.com/tags/att_input_size.asp">w3schools.com</see>.
    /// </summary>
    [Parameter] public int? VisibleCharacters { get; set; }

    /// <summary>
    /// If true, step buttons will be visible.
    /// </summary>
    [Parameter] public bool? ShowStepButtons { get; set; }

    /// <summary>
    /// If true, enables change of numeric value by pressing on step buttons or by keyboard up/down keys.
    /// </summary>
    [Parameter] public bool? EnableStep { get; set; }

    /// <summary>
    /// If true, selects all the text entered in the input field once it receives the focus.
    /// </summary>
    [Parameter] public bool SelectAllOnFocus { get; set; }

    /// <summary>
    /// Determine if the element value can be incremented / decremented with the mouse wheel.
    /// </summary>
    [Parameter] public bool ModifyValueOnWheel { get; set; }

    /// <summary>
    /// Used in conjonction with the <see cref="ModifyValueOnWheel"/> option, defines when the wheel event will increment or decrement the element value, either when the element is focused, or hovered.
    /// </summary>
    [Parameter] public NumericWheelOn WheelOn { get; set; }

    #endregion
}