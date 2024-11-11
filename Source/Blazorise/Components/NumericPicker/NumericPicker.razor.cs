#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Numerics;
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
/// <typeparam name="TValue">Data-type to be binded by the <see cref="Value"/> property.</typeparam>
public partial class NumericPicker<TValue> : BaseTextInput<TValue>, INumericPicker, IAsyncDisposable
{
    #region Members

    /// <summary>
    /// Object reference that can be accessed through the JSInterop.
    /// </summary>
    private DotNetObjectReference<NumericPickerAdapter> dotNetObjectRef;

    /// <summary>
    /// Indicates if <see cref="Min"/> parameter is defined.
    /// </summary>
    private bool MinDefined = false;

    /// <summary>
    /// Indicates if <see cref="Max"/> parameter is defined.
    /// </summary>
    private bool MaxDefined = false;

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
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        if ( Rendered )
        {
            var decimalsChanged = isIntegerType ? false : parameters.TryGetValue<int>( nameof( Decimals ), out var paramDecimals ) && !Decimals.IsEqual( paramDecimals );
            var decimalSeparatorChanged = parameters.TryGetValue<string>( nameof( DecimalSeparator ), out var paramDecimalSeparator ) && !DecimalSeparator.IsEqual( paramDecimalSeparator );
            var alternativeDecimalSeparatorChanged = parameters.TryGetValue<string>( nameof( AlternativeDecimalSeparator ), out var paramAlternativeDecimalSeparator ) && !AlternativeDecimalSeparator.IsEqual( paramAlternativeDecimalSeparator );

            var groupSeparatorChanged = parameters.TryGetValue<string>( nameof( GroupSeparator ), out var paramGroupSeparator ) && !GroupSeparator.IsEqual( paramGroupSeparator );
            var groupSpacingChanged = parameters.TryGetValue<string>( nameof( GroupSpacing ), out var paramGroupSpacing ) && !GroupSpacing.IsEqual( paramGroupSpacing );

            var currencySymbolChanged = parameters.TryGetValue<string>( nameof( CurrencySymbol ), out var paramCurrencySymbol ) && !CurrencySymbol.IsEqual( paramCurrencySymbol );
            var currencySymbolPlacementChanged = parameters.TryGetValue<CurrencySymbolPlacement>( nameof( CurrencySymbolPlacement ), out var paramCurrencySymbolPlacement ) && !CurrencySymbolPlacement.IsEqual( paramCurrencySymbolPlacement );

            var roundingMethodChanged = parameters.TryGetValue<NumericRoundingMethod>( nameof( RoundingMethod ), out var paramRoundingMethod ) && !RoundingMethod.IsEqual( paramRoundingMethod );

            var minChanged = parameters.TryGetValue<TValue>( nameof( Min ), out var paramMin ) && !Min.IsEqual( paramMin );
            var maxChanged = parameters.TryGetValue<TValue>( nameof( Max ), out var paramMax ) && !Max.IsEqual( paramMax );
            var minMaxLimitsOverrideChanged = parameters.TryGetValue<NumericMinMaxLimitsOverride>( nameof( MinMaxLimitsOverride ), out var paramMinMaxLimitsOverride ) && !MinMaxLimitsOverride.IsEqual( paramMinMaxLimitsOverride );

            var selectAllOnFocusChanged = parameters.TryGetValue<bool>( nameof( SelectAllOnFocus ), out var paramSelectAllOnFocus ) && !SelectAllOnFocus.IsEqual( paramSelectAllOnFocus );

            var allowDecimalPaddingChanged = parameters.TryGetValue<NumericAllowDecimalPadding>( nameof( AllowDecimalPadding ), out var paramAllowDecimalPadding ) && !AllowDecimalPadding.IsEqual( paramAllowDecimalPadding );
            var alwaysAllowDecimalSeparatorChanged = parameters.TryGetValue<bool>( nameof( AlwaysAllowDecimalSeparator ), out var paramAlwaysAllowDecimalSeparator ) && !AlwaysAllowDecimalSeparator.IsEqual( paramAlwaysAllowDecimalSeparator );

            var modifyValueOnWheelChanged = parameters.TryGetValue<bool>( nameof( ModifyValueOnWheel ), out var paramModifyValueOnWheel ) && !ModifyValueOnWheel.IsEqual( paramModifyValueOnWheel );
            var wheelOnChanged = parameters.TryGetValue<NumericWheelOn>( nameof( WheelOn ), out var paramWheelOn ) && !WheelOn.IsEqual( paramWheelOn );

            if ( decimalsChanged || decimalSeparatorChanged || alternativeDecimalSeparatorChanged
                || groupSeparatorChanged || groupSpacingChanged
                || currencySymbolChanged || currencySymbolPlacementChanged
                || roundingMethodChanged
                || minChanged || maxChanged
                || selectAllOnFocusChanged
                || allowDecimalPaddingChanged || alwaysAllowDecimalSeparatorChanged
                || modifyValueOnWheelChanged )
            {
                ExecuteAfterRender(async () => await JSModule.UpdateOptions(ElementRef, ElementId, new NumericPickerUpdateJSOptions
                {
                    Decimals = new JSOptionChange<int>(decimalsChanged, GetDecimals()),
                    DecimalSeparator = new JSOptionChange<string>(decimalSeparatorChanged, paramDecimalSeparator),
                    AlternativeDecimalSeparator = new JSOptionChange<string>(alternativeDecimalSeparatorChanged, paramAlternativeDecimalSeparator),
                    GroupSeparator = new JSOptionChange<string>(groupSeparatorChanged, paramGroupSeparator),
                    GroupSpacing = new JSOptionChange<string>(groupSpacingChanged, paramGroupSpacing),
                    CurrencySymbol = new JSOptionChange<string>(currencySymbolChanged, paramCurrencySymbol),
                    CurrencySymbolPlacement = new JSOptionChange<string>(currencySymbolPlacementChanged, paramCurrencySymbolPlacement.ToCurrencySymbolPlacement()),
                    RoundingMethod = new JSOptionChange<string>(roundingMethodChanged, paramRoundingMethod.ToNumericRoundingMethod()),
                    AllowDecimalPadding = new JSOptionChange<object>(allowDecimalPaddingChanged, paramAllowDecimalPadding.ToNumericDecimalPadding()),
                    AlwaysAllowDecimalSeparator = new JSOptionChange<bool>(alwaysAllowDecimalSeparatorChanged, paramAlwaysAllowDecimalSeparator),
                    Min = new JSOptionChange<object>(minChanged, paramMin),
                    Max = new JSOptionChange<object>(maxChanged, paramMax),
                    MinMaxLimitsOverride = new JSOptionChange<object>(minMaxLimitsOverrideChanged, paramMinMaxLimitsOverride),
                    SelectAllOnFocus = new JSOptionChange<bool>(selectAllOnFocusChanged, paramSelectAllOnFocus),
                    ModifyValueOnWheel = new JSOptionChange<bool>(modifyValueOnWheelChanged, paramModifyValueOnWheel),
                    WheelOn = new JSOptionChange<object>(wheelOnChanged, paramWheelOn.ToNumericWheelOn()),
                }));

            }
        }

        var valueChanged = parameters.TryGetValue<TValue>( nameof( Value ), out var paramValue ) && !Value.IsEqual( paramValue );

        if ( valueChanged )
        {
            ExecuteAfterRender( async () => await JSModule.UpdateValue( ElementRef, ElementId, paramValue ) );
        }

        // This make sure we know that Min or Max parameters are defined and can be checked against the current value.
        // Without we cannot determine if Min or Max has a default value when TValue is non-nullable type.
        MinDefined = parameters.TryGetValue<TValue>( nameof( Min ), out var min );
        MaxDefined = parameters.TryGetValue<TValue>( nameof( Max ), out var max );

        await base.SetParametersAsync( parameters );

        if ( ParentValidation is not null )
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
    protected override async Task OnFirstAfterRenderAsync()
    {
        dotNetObjectRef ??= CreateDotNetObjectRef( new NumericPickerAdapter( this ) );

        // find the min and max possible value based on the supplied value type
        var (minFromType, maxFromType) = Converters.GetMinMaxValueOfType<TValue>();

        await JSModule.Initialize(dotNetObjectRef, ElementRef, ElementId, new NumericPickerInitializeJSOptions
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
        });

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
    /// Allow padding the decimal places with zeros. If set to <c>Floats</c>, padding is only done when there are some decimals.        /// 
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
    /// Override the minimum and maximum limits.
    /// </summary>
    [Parameter] public NumericMinMaxLimitsOverride MinMaxLimitsOverride { get; set; } = NumericMinMaxLimitsOverride.Ignore;

    /// <summary>
    /// The size attribute specifies the visible width, in characters, of an input element. https://www.w3schools.com/tags/att_input_size.asp
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