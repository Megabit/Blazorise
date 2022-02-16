#region Using directives
using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Modules;
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
    public partial class NumericEdit<TValue> : BaseTextInput<TValue>, IAsyncDisposable
    {
        #region Methods

        /// <inheritdoc/>
        public override async Task SetParametersAsync( ParameterView parameters )
        {
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
            builder.Append( ClassProvider.NumericEditSize( ThemeSize ), ThemeSize != Blazorise.Size.Default );
            builder.Append( ClassProvider.NumericEditColor( Color ), Color != Color.Default );
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
}
