#region Using directives
using System;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Modules;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise;

/// <summary>
/// A slider to select a range of values.
/// </summary>
/// <typeparam name="TValue">Data-type used by the slider handles.</typeparam>
public partial class RangeSlider<TValue> : BaseInputComponent<RangeSliderValue<TValue>, RangeSliderClasses, RangeSliderStyles>, IAsyncDisposable
{
    #region Members

    private const string RANGE_DELIMITER = "|";

    private bool startHandleActive = true;

    /// <summary>
    /// Represents a reference to the end element in the current context.
    /// </summary>
    protected ElementReference EndElementRef;

    /// <summary>
    /// Captured Step parameter snapshot.
    /// </summary>
    protected ComponentParameterInfo<TValue> paramStep;

    /// <summary>
    /// Captured Min parameter snapshot.
    /// </summary>
    protected ComponentParameterInfo<TValue> paramMin;

    /// <summary>
    /// Captured Max parameter snapshot.
    /// </summary>
    protected ComponentParameterInfo<TValue> paramMax;

    /// <summary>
    /// Captured ClampToOtherHandle parameter snapshot.
    /// </summary>
    protected ComponentParameterInfo<bool> paramClampToOtherHandle;

    /// <summary>
    /// Captured AllowEqualValues parameter snapshot.
    /// </summary>
    protected ComponentParameterInfo<bool> paramAllowEqualValues;

    #endregion

    #region Constructors

    /// <summary>
    /// A default <see cref="RangeSlider{TValue}"/> constructor.
    /// </summary>
    public RangeSlider()
    {
        TrackClassBuilder = new( BuildTrackClasses, builder => builder.Append( Classes?.Track ) );
        RangeClassBuilder = new( BuildRangeClasses, builder => builder.Append( Classes?.Range ) );
        StartInputClassBuilder = new( BuildStartInputClasses, builder =>
        {
            builder.Append( Classes?.Input );
            builder.Append( Classes?.StartInput );
        } );
        EndInputClassBuilder = new( BuildEndInputClasses, builder =>
        {
            builder.Append( Classes?.Input );
            builder.Append( Classes?.EndInput );
        } );
        TooltipClassBuilder = new( BuildTooltipClasses, builder => builder.Append( Classes?.Tooltip ) );
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void CaptureParameters( ParameterView parameters )
    {
        base.CaptureParameters( parameters );

        parameters.TryGetParameter( Step, out paramStep );
        parameters.TryGetParameter( Min, out paramMin );
        parameters.TryGetParameter( Max, out paramMax );
        parameters.TryGetParameter( ClampToOtherHandle, out paramClampToOtherHandle );
        parameters.TryGetParameter( AllowEqualValues, out paramAllowEqualValues );
    }

    /// <inheritdoc/>
    protected override async Task OnBeforeSetParametersAsync( ParameterView parameters )
    {
        await base.OnBeforeSetParametersAsync( parameters );

        if ( Rendered
            && ( paramClampToOtherHandle.Defined && paramClampToOtherHandle.Changed
                || paramAllowEqualValues.Defined && paramAllowEqualValues.Changed ) )
        {
            bool clampToOtherHandle = paramClampToOtherHandle.Defined ? paramClampToOtherHandle.Value : ClampToOtherHandle;
            bool allowEqualValues = paramAllowEqualValues.Defined ? paramAllowEqualValues.Value : AllowEqualValues;

            ExecuteAfterRender( () => JSModule.Initialize( ElementRef, ElementId, EndElementRef, EndElementId, clampToOtherHandle, allowEqualValues ).AsTask() );
        }
    }

    /// <inheritdoc/>
    protected override async Task OnFirstAfterRenderAsync()
    {
        await JSModule.Initialize( ElementRef, ElementId, EndElementRef, EndElementId, ClampToOtherHandle, AllowEqualValues );

        await base.OnFirstAfterRenderAsync();
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing && Rendered )
        {
            await JSModule.SafeDestroy( ElementRef, ElementId );
        }

        await base.DisposeAsync( disposing );
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.RangeSlider() );

        base.BuildClasses( builder );
    }

    /// <summary>
    /// Builds class names for the inactive range track.
    /// </summary>
    /// <param name="builder">Class builder used to append class names.</param>
    private void BuildTrackClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.RangeSliderTrack() );
    }

    /// <summary>
    /// Builds class names for the selected range track.
    /// </summary>
    /// <param name="builder">Class builder used to append class names.</param>
    private void BuildRangeClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.RangeSliderRange() );
    }

    /// <summary>
    /// Builds class names for start input.
    /// </summary>
    /// <param name="builder">Class builder used to append class names.</param>
    private void BuildStartInputClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.RangeSliderInput() );
        builder.Append( ClassProvider.RangeSliderStart() );
        builder.Append( ClassProvider.RangeSliderValidation( ParentValidation?.Status ?? ValidationStatus.None ) );
    }

    /// <summary>
    /// Builds class names for end input.
    /// </summary>
    /// <param name="builder">Class builder used to append class names.</param>
    private void BuildEndInputClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.RangeSliderInput() );
        builder.Append( ClassProvider.RangeSliderEnd() );
        builder.Append( ClassProvider.RangeSliderValidation( ParentValidation?.Status ?? ValidationStatus.None ) );
    }

    /// <summary>
    /// Builds class names for range value tooltips.
    /// </summary>
    /// <param name="builder">Class builder used to append class names.</param>
    private void BuildTooltipClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.RangeSliderTooltip() );
    }

    /// <inheritdoc/>
    protected internal override void DirtyClasses()
    {
        TrackClassBuilder.Dirty();
        RangeClassBuilder.Dirty();
        StartInputClassBuilder.Dirty();
        EndInputClassBuilder.Dirty();
        TooltipClassBuilder.Dirty();

        base.DirtyClasses();
    }

    /// <inheritdoc/>
    protected override bool IsSameAsInternalValue( RangeSliderValue<TValue> value )
    {
        return value.Start.IsEqual( Value.Start )
               && value.End.IsEqual( Value.End );
    }

    /// <inheritdoc/>
    protected override Task<ParseValue<RangeSliderValue<TValue>>> ParseValueFromStringAsync( string value )
    {
        if ( string.IsNullOrEmpty( value ) )
            return Task.FromResult( ParseValue<RangeSliderValue<TValue>>.Empty );

        string[] values = value.Split( RANGE_DELIMITER, StringSplitOptions.None );

        if ( values.Length != 2
             || !TryParseSingleValue( values[0], out TValue startValue )
             || !TryParseSingleValue( values[1], out TValue endValue ) )
        {
            return Task.FromResult( ParseValue<RangeSliderValue<TValue>>.Empty );
        }

        RangeSliderValue<TValue> parsedRange = NormalizeRange( startValue, endValue );

        return Task.FromResult( new ParseValue<RangeSliderValue<TValue>>( true, parsedRange, null ) );
    }

    /// <inheritdoc/>
    protected override string FormatValueAsString( RangeSliderValue<TValue> value )
        => $"{FormatSingleValue( value.Start )}{RANGE_DELIMITER}{FormatSingleValue( value.End )}";

    /// <summary>
    /// Handles the start handle oninput event.
    /// </summary>
    protected Task OnStartInputHandler( ChangeEventArgs eventArgs )
    {
        return HandleInputAsync( eventArgs?.Value?.ToString(), true );
    }

    /// <summary>
    /// Handles the end handle oninput event.
    /// </summary>
    protected Task OnEndInputHandler( ChangeEventArgs eventArgs )
    {
        return HandleInputAsync( eventArgs?.Value?.ToString(), false );
    }

    /// <summary>
    /// Handles pointer down event on start handle.
    /// </summary>
    protected Task OnStartPointerDownHandler( PointerEventArgs eventArgs )
    {
        if ( !startHandleActive )
        {
            startHandleActive = true;
            DirtyStyles();
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles pointer down event on end handle.
    /// </summary>
    protected Task OnEndPointerDownHandler( PointerEventArgs eventArgs )
    {
        if ( startHandleActive )
        {
            startHandleActive = false;
            DirtyStyles();
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles focus event on start handle.
    /// </summary>
    protected Task OnStartFocusHandler( FocusEventArgs eventArgs )
    {
        if ( !startHandleActive )
        {
            startHandleActive = true;
            DirtyStyles();
        }

        return OnFocusHandler( eventArgs );
    }

    /// <summary>
    /// Handles focus event on end handle.
    /// </summary>
    protected Task OnEndFocusHandler( FocusEventArgs eventArgs )
    {
        if ( startHandleActive )
        {
            startHandleActive = false;
            DirtyStyles();
        }

        return OnFocusHandler( eventArgs );
    }

    private Task HandleInputAsync( string value, bool startHandle )
    {
        if ( string.IsNullOrEmpty( value ) )
            return Task.CompletedTask;

        if ( !TryParseSingleValue( value, out TValue parsedValue )
             || !TryConvertToDouble( parsedValue, out double parsedNumber ) )
        {
            return Task.CompletedTask;
        }

        RangeSliderValue<TValue> currentRange = CurrentNormalizedRange;
        double currentStartValue = GetValueAsDouble( currentRange.Start, parsedNumber );
        double currentEndValue = GetValueAsDouble( currentRange.End, parsedNumber );
        double minimumDistance = AllowEqualValues ? 0d : GetStepDistance();

        double nextStartValue = startHandle ? parsedNumber : currentStartValue;
        double nextEndValue = startHandle ? currentEndValue : parsedNumber;

        if ( ClampToOtherHandle )
        {
            if ( startHandle )
                nextStartValue = Math.Min( nextStartValue, currentEndValue - minimumDistance );
            else
                nextEndValue = Math.Max( nextEndValue, currentStartValue + minimumDistance );
        }

        RangeSliderValue<TValue> nextRange = NormalizeRange( nextStartValue, nextEndValue );

        return CurrentValueHandler( FormatValueAsString( nextRange ) );
    }

    private RangeSliderValue<TValue> NormalizeRange( TValue startValue, TValue endValue )
    {
        double minimum = 0d;
        bool hasMinimum = MinDefined && TryConvertToDouble( Min, out minimum );

        double maximum = 0d;
        bool hasMaximum = MaxDefined && TryConvertToDouble( Max, out maximum );

        if ( hasMinimum && hasMaximum && minimum > maximum )
            (minimum, maximum) = (maximum, minimum);

        double fallbackStartValue = hasMinimum ? minimum : 0d;
        double fallbackEndValue = hasMaximum ? maximum : fallbackStartValue;

        double startNumber = GetValueAsDouble( startValue, fallbackStartValue );
        double endNumber = GetValueAsDouble( endValue, fallbackEndValue );

        return NormalizeRange( startNumber, endNumber );
    }

    private RangeSliderValue<TValue> NormalizeRange( double startValue, double endValue )
    {
        if ( startValue > endValue )
            (startValue, endValue) = (endValue, startValue);

        double minimum = 0d;
        bool hasMinimum = MinDefined && TryConvertToDouble( Min, out minimum );

        double maximum = 0d;
        bool hasMaximum = MaxDefined && TryConvertToDouble( Max, out maximum );

        if ( hasMinimum && hasMaximum && minimum > maximum )
            (minimum, maximum) = (maximum, minimum);

        if ( hasMinimum )
        {
            startValue = Math.Max( startValue, minimum );
            endValue = Math.Max( endValue, minimum );
        }

        if ( hasMaximum )
        {
            startValue = Math.Min( startValue, maximum );
            endValue = Math.Min( endValue, maximum );
        }

        if ( startValue > endValue )
            (startValue, endValue) = (endValue, startValue);

        if ( !TryConvertFromDouble( startValue, out TValue normalizedStartValue )
             || !TryConvertFromDouble( endValue, out TValue normalizedEndValue ) )
        {
            throw new InvalidOperationException( $"Unsupported type {typeof( TValue )}" );
        }

        return new RangeSliderValue<TValue>( normalizedStartValue, normalizedEndValue );
    }

    private void GetDisplayBounds( RangeSliderValue<TValue> range, out double minimum, out double maximum )
    {
        minimum = 0d;
        bool hasMinimum = MinDefined && TryConvertToDouble( Min, out minimum );

        maximum = 0d;
        bool hasMaximum = MaxDefined && TryConvertToDouble( Max, out maximum );

        double startValue = GetValueAsDouble( range.Start, hasMinimum ? minimum : 0d );
        double endValue = GetValueAsDouble( range.End, hasMaximum ? maximum : startValue );

        if ( !hasMinimum )
            minimum = Math.Min( startValue, endValue );

        if ( !hasMaximum )
            maximum = Math.Max( startValue, endValue );

        if ( minimum > maximum )
            (minimum, maximum) = (maximum, minimum);
    }

    private static bool TryParseSingleValue( string value, out TValue result )
    {
        return Converters.TryChangeType<TValue>( value, out result, CultureInfo.InvariantCulture );
    }

    private static bool TryConvertToDouble( TValue value, out double result )
    {
        return Converters.TryChangeType<double>( value, out result, CultureInfo.InvariantCulture );
    }

    private static bool TryConvertFromDouble( double value, out TValue result )
    {
        if ( Converters.TryChangeType<TValue>( value, out result, CultureInfo.InvariantCulture ) )
            return true;

        return Converters.TryChangeType<TValue>( value.ToString( "G17", CultureInfo.InvariantCulture ), out result, CultureInfo.InvariantCulture );
    }

    private static double GetValueAsDouble( TValue value, double fallback )
    {
        if ( TryConvertToDouble( value, out double number ) )
            return number;

        return fallback;
    }

    private static string FormatSingleValue( TValue value )
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

    private static double CalculatePercentage( double value, double minimum, double maximum )
    {
        if ( maximum <= minimum )
            return 0d;

        return Math.Clamp( ( value - minimum ) / ( maximum - minimum ) * 100d, 0d, 100d );
    }

    private static string ToPercentageString( double value )
    {
        return $"{value.ToString( "0.####", CultureInfo.InvariantCulture )}%";
    }

    private double GetStepDistance()
    {
        if ( StepDefined && TryConvertToDouble( Step, out double step ) && step > 0d )
            return step;

        return 1d;
    }

    private static string JoinStyles( params string[] styles )
    {
        StringBuilder stringBuilder = null;

        foreach ( string style in styles )
        {
            if ( string.IsNullOrWhiteSpace( style ) )
                continue;

            stringBuilder ??= new StringBuilder();

            string trimmedStyle = style.Trim().TrimEnd( ';' );

            if ( stringBuilder.Length > 0 )
                stringBuilder.Append( "; " );

            stringBuilder.Append( trimmedStyle );
        }

        return stringBuilder?.ToString();
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    private RangeSliderValue<TValue> CurrentNormalizedRange
        => NormalizeRange( Value.Start, Value.End );

    private double StartPercentage
    {
        get
        {
            RangeSliderValue<TValue> range = CurrentNormalizedRange;
            GetDisplayBounds( range, out double minimum, out double maximum );

            double startValue = GetValueAsDouble( range.Start, minimum );

            return CalculatePercentage( startValue, minimum, maximum );
        }
    }

    private double EndPercentage
    {
        get
        {
            RangeSliderValue<TValue> range = CurrentNormalizedRange;
            GetDisplayBounds( range, out double minimum, out double maximum );

            double endValue = GetValueAsDouble( range.End, maximum );

            return CalculatePercentage( endValue, minimum, maximum );
        }
    }

    /// <summary>
    /// Gets the selected range start value represented as a string.
    /// </summary>
    protected string StartValueString
        => FormatSingleValue( CurrentNormalizedRange.Start );

    /// <summary>
    /// Gets the selected range end value represented as a string.
    /// </summary>
    protected string EndValueString
        => FormatSingleValue( CurrentNormalizedRange.End );

    /// <summary>
    /// Gets the string representation of the <see cref="Step"/> value.
    /// </summary>
    protected string StepValueString
        => StepDefined ? FormatSingleValue( Step ) : null;

    /// <summary>
    /// Gets the string representation of the <see cref="Min"/> value.
    /// </summary>
    protected string MinValueString
        => MinDefined ? FormatSingleValue( Min ) : null;

    /// <summary>
    /// Gets the string representation of the <see cref="Max"/> value.
    /// </summary>
    protected string MaxValueString
        => MaxDefined ? FormatSingleValue( Max ) : null;

    /// <summary>
    /// Indicates if <see cref="Step"/> parameter is defined.
    /// </summary>
    protected bool StepDefined
        => paramStep.Defined;

    /// <summary>
    /// Indicates if <see cref="Min"/> parameter is defined.
    /// </summary>
    protected bool MinDefined
        => paramMin.Defined;

    /// <summary>
    /// Indicates if <see cref="Max"/> parameter is defined.
    /// </summary>
    protected bool MaxDefined
        => paramMax.Defined;

    /// <summary>
    /// Gets the generated id of the end range input element.
    /// </summary>
    protected string EndElementId
        => $"{ElementId}_end";

    /// <summary>
    /// Gets the generated name of the start range input element.
    /// </summary>
    protected string StartNameAttributeValue
        => string.IsNullOrEmpty( NameAttributeValue ) ? null : $"{NameAttributeValue}.Start";

    /// <summary>
    /// Gets the generated name of the end range input element.
    /// </summary>
    protected string EndNameAttributeValue
        => string.IsNullOrEmpty( NameAttributeValue ) ? null : $"{NameAttributeValue}.End";

    /// <summary>
    /// Gets the class names for the inactive range track element.
    /// </summary>
    protected string TrackClassNames
        => TrackClassBuilder.Class;

    /// <summary>
    /// Gets the class names for the selected range track element.
    /// </summary>
    protected string RangeClassNames
        => RangeClassBuilder.Class;

    /// <summary>
    /// Gets the class names for the start range input.
    /// </summary>
    protected string StartInputClassNames
        => StartInputClassBuilder.Class;

    /// <summary>
    /// Gets the class names for the end range input.
    /// </summary>
    protected string EndInputClassNames
        => EndInputClassBuilder.Class;

    /// <summary>
    /// Gets the class names for tooltip elements.
    /// </summary>
    protected string TooltipClassNames
        => TooltipClassBuilder.Class;

    /// <summary>
    /// Gets the style names for the inactive range track element.
    /// </summary>
    protected string TrackStyleNames
        => JoinStyles( Styles?.Track );

    /// <summary>
    /// Gets the style names for the selected range track element.
    /// </summary>
    protected string RangeStyleNames
    {
        get
        {
            double startPercentage = StartPercentage;
            double endPercentage = EndPercentage;
            double rangeWidth = Math.Max( endPercentage - startPercentage, 0d );

            return JoinStyles(
                Styles?.Range,
                $"left: {ToPercentageString( startPercentage )}",
                $"width: {ToPercentageString( rangeWidth )}" );
        }
    }

    /// <summary>
    /// Gets the style names for the start range input.
    /// </summary>
    protected string StartInputStyleNames
        => JoinStyles(
            Styles?.Input,
            Styles?.StartInput,
            startHandleActive ? "z-index: 5" : "z-index: 4" );

    /// <summary>
    /// Gets the style names for the end range input.
    /// </summary>
    protected string EndInputStyleNames
        => JoinStyles(
            Styles?.Input,
            Styles?.EndInput,
            startHandleActive ? "z-index: 4" : "z-index: 5" );

    /// <summary>
    /// Gets the style names for the start tooltip.
    /// </summary>
    protected string StartTooltipStyleNames
        => JoinStyles(
            Styles?.Tooltip,
            $"left: {ToPercentageString( StartPercentage )}" );

    /// <summary>
    /// Gets the style names for the end tooltip.
    /// </summary>
    protected string EndTooltipStyleNames
        => JoinStyles(
            Styles?.Tooltip,
            $"left: {ToPercentageString( EndPercentage )}" );

    /// <summary>
    /// Class builder for inactive track.
    /// </summary>
    protected ClassBuilder TrackClassBuilder { get; private set; }

    /// <summary>
    /// Class builder for selected range.
    /// </summary>
    protected ClassBuilder RangeClassBuilder { get; private set; }

    /// <summary>
    /// Class builder for start input.
    /// </summary>
    protected ClassBuilder StartInputClassBuilder { get; private set; }

    /// <summary>
    /// Class builder for end input.
    /// </summary>
    protected ClassBuilder EndInputClassBuilder { get; private set; }

    /// <summary>
    /// Class builder for tooltip elements.
    /// </summary>
    protected ClassBuilder TooltipClassBuilder { get; private set; }

    /// <summary>
    /// Specifies if the value tooltips are visible.
    /// </summary>
    [Parameter] public bool ShowValueTooltips { get; set; } = true;

    /// <summary>
    /// Specifies the <see cref="IJSRangeSliderModule"/> instance.
    /// </summary>
    [Inject] public IJSRangeSliderModule JSModule { get; set; }

    /// <summary>
    /// Accessible label text for the start handle.
    /// </summary>
    [Parameter] public string StartAriaLabel { get; set; } = "Start value";

    /// <summary>
    /// Accessible label text for the end handle.
    /// </summary>
    [Parameter] public string EndAriaLabel { get; set; } = "End value";

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

    /// <summary>
    /// Specifies whether the active handle should stop at the other handle while dragging.
    /// </summary>
    /// <remarks>
    /// When set to <c>true</c>, the active handle is clamped to the other handle instead of swapping positions.
    /// </remarks>
    [Parameter] public bool ClampToOtherHandle { get; set; }

    /// <summary>
    /// Specifies whether both handles can point to the same value.
    /// </summary>
    /// <remarks>
    /// When set to <c>false</c> and <see cref="ClampToOtherHandle"/> is enabled, the handles keep at least one <see cref="Step"/> between them.
    /// </remarks>
    [Parameter] public bool AllowEqualValues { get; set; } = true;

    #endregion
}