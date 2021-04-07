#region Using directives
using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
#endregion

namespace Blazorise
{
    /// <summary>
    /// An editor that displays a numeric value and allows a user to edit the value.
    /// </summary>
    /// <typeparam name="TValue">Data-type to be binded by the <see cref="Value"/> property.</typeparam>
    public partial class NumericEdit<TValue> : BaseTextInput<TValue>, INumericEdit
    {
        #region Members

        // taken from https://github.com/aspnet/AspNetCore/issues/11159
        private DotNetObjectReference<NumericEditAdapter> dotNetObjectRef;

        #endregion

        #region Methods

        /// <inheritdoc/>
        public override async Task SetParametersAsync( ParameterView parameters )
        {
            var decimalsChanged = parameters.TryGetValue( nameof( Decimals ), out int decimals ) && !Decimals.IsEqual( decimals );

            if ( Rendered && decimalsChanged )
            {
                ExecuteAfterRender( async () => await JSRunner.UpdateNumericEdit( ElementRef, ElementId, new
                {
                    Decimals = new { Changed = decimalsChanged, Value = decimals },
                } ) );
            }

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
        protected override async Task OnFirstAfterRenderAsync()
        {
            dotNetObjectRef ??= CreateDotNetObjectRef( new NumericEditAdapter( this ) );

            // find the min and max possible value based on the supplied value type
            var (minFromType, maxFromType) = Converters.GetMinMaxValueOfType<TValue>();

            await JSRunner.InitializeNumericEdit<TValue>( dotNetObjectRef, ElementRef, ElementId, new
            {
                Decimals = Decimals,
                Separator = DecimalsSeparator,
                Step,
                Min = Min.IsEqual( default ) ? minFromType : Min,
                Max = Max.IsEqual( default ) ? maxFromType : Max
            } );

            await base.OnFirstAfterRenderAsync();
        }

        /// <inheritdoc/>
        protected override async ValueTask DisposeAsync( bool disposing )
        {
            if ( disposing && Rendered )
            {
                var task = JSRunner.DestroyNumericEdit( ElementRef, ElementId );

                try
                {
                    await task;
                }
                catch
                {
                    if ( !task.IsCanceled )
                    {
                        throw;
                    }
                }

                DisposeDotNetObjectRef( dotNetObjectRef );
            }

            await base.DisposeAsync( disposing );
        }

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.NumericEdit( Plaintext ) );
            builder.Append( ClassProvider.NumericEditSize( Size ), Size != Size.None );
            builder.Append( ClassProvider.NumericEditColor( Color ), Color != Color.None );
            builder.Append( ClassProvider.NumericEditValidation( ParentValidation?.Status ?? ValidationStatus.None ), ParentValidation?.Status != ValidationStatus.None );

            base.BuildClasses( builder );
        }

        /// <inheritdoc/>
        public Task SetValue( string value )
        {
            return CurrentValueHandler( value );
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
            switch ( value )
            {
                case null:
                    return null;
                case byte @byte:
                    return Converters.FormatValue( @byte, CurrentCultureInfo );
                case short @short:
                    return Converters.FormatValue( @short, CurrentCultureInfo );
                case int @int:
                    return Converters.FormatValue( @int, CurrentCultureInfo );
                case long @long:
                    return Converters.FormatValue( @long, CurrentCultureInfo );
                case float @float:
                    return Converters.FormatValue( @float, CurrentCultureInfo );
                case double @double:
                    return Converters.FormatValue( @double, CurrentCultureInfo );
                case decimal @decimal:
                    return Converters.FormatValue( @decimal, CurrentCultureInfo );
                case sbyte @sbyte:
                    return Converters.FormatValue( @sbyte, CurrentCultureInfo );
                case ushort @ushort:
                    return Converters.FormatValue( @ushort, CurrentCultureInfo );
                case uint @uint:
                    return Converters.FormatValue( @uint, CurrentCultureInfo );
                case ulong @ulong:
                    return Converters.FormatValue( @ulong, CurrentCultureInfo );
                default:
                    throw new InvalidOperationException( $"Unsupported type {value.GetType()}" );
            }
        }

        /// <inheritdoc/>
        protected override async Task OnBlurHandler( FocusEventArgs eventArgs )
        {
            await base.OnBlurHandler( eventArgs );

            if ( !string.IsNullOrEmpty( CurrentValueAsString )
                && CurrentValue is IComparable number
                && number != null )
            {
                var defaultValue = DefaultValue as IComparable;

                // We still need to allow for default value to be entered.
                // - Non nullable value: 0 or empty
                // - Nullable value:     null or empty
                if ( number.CompareTo( defaultValue ) != 0 )
                {
                    if ( Max is IComparable max && max.CompareTo( defaultValue ) != 0 && number.CompareTo( max ) > 0 )
                    {
                        number = max;
                    }
                    else if ( Min is IComparable min && min.CompareTo( defaultValue ) != 0 && number.CompareTo( min ) < 0 )
                    {
                        number = min;
                    }

                    // cast back to TValue and check if number has changed
                    if ( Converters.TryChangeType<TValue>( number, out var currentValue, CurrentCultureInfo )
                        && !CurrentValue.IsEqual( currentValue ) )
                    {
                        // number has changed so we need to re-set the CurrentValue and re-run any validation
                        await CurrentValueHandler( FormatValueAsString( currentValue ) );
                    }
                }
            }
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
                // TODO: find the right culture based on DecimalsSeparator
                if ( !string.IsNullOrEmpty( Culture ) )
                {
                    return CultureInfo.GetCultureInfo( Culture );
                }

                return CultureInfo.InvariantCulture;
            }
        }

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
        [Parameter] public decimal? Step { get; set; }

        /// <summary>
        /// Maximum number of decimal places after the decimal separator.
        /// </summary>
        [Parameter] public int Decimals { get; set; } = 2;

        /// <summary>
        /// String to use as the decimal separator in numeric values.
        /// </summary>
        [Parameter] public string DecimalsSeparator { get; set; } = ".";

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
}
