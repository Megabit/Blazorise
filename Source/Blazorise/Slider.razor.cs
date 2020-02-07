#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Threading.Tasks;
using Blazorise.Utils;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class Slider<TValue> : BaseInputComponent<TValue>
    {
        #region Members

        //private Color color = Color.None;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Slider() );
            //builder.Append( ClassProvider.SliderColor( Color ), Color != Color.None );

            base.BuildClasses( builder );
        }

        protected virtual Task OnChangeHandler( ChangeEventArgs e )
        {
            return CurrentValueHandler( e?.Value?.ToString() );
        }

        protected override Task OnInternalValueChanged( TValue value )
        {
            return ValueChanged.InvokeAsync( value );
        }

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

        protected override string FormatValueAsString( TValue value )
        {
            switch ( value )
            {
                case null:
                    return null;
                case byte @byte:
                    return Converters.FormatValue( @byte, CultureInfo.InvariantCulture );
                case short @short:
                    return Converters.FormatValue( @short, CultureInfo.InvariantCulture );
                case int @int:
                    return Converters.FormatValue( @int, CultureInfo.InvariantCulture );
                case long @long:
                    return Converters.FormatValue( @long, CultureInfo.InvariantCulture );
                case float @float:
                    return Converters.FormatValue( @float, CultureInfo.InvariantCulture );
                case double @double:
                    return Converters.FormatValue( @double, CultureInfo.InvariantCulture );
                case decimal @decimal:
                    return Converters.FormatValue( @decimal, CultureInfo.InvariantCulture );
                case sbyte @sbyte:
                    return Converters.FormatValue( @sbyte, CultureInfo.InvariantCulture );
                case ushort @ushort:
                    return Converters.FormatValue( @ushort, CultureInfo.InvariantCulture );
                case uint @uint:
                    return Converters.FormatValue( @uint, CultureInfo.InvariantCulture );
                case ulong @ulong:
                    return Converters.FormatValue( @ulong, CultureInfo.InvariantCulture );
                default:
                    throw new InvalidOperationException( $"Unsupported type {value.GetType()}" );
            }
        }

        #endregion

        #region Properties

        [Inject] protected BlazoriseOptions Options { get; set; }

        protected override TValue InternalValue { get => Value; set => Value = value; }

        ///// <summary>
        ///// Gets or sets the tick color.
        ///// </summary>
        //[Parameter]
        //public Color Color
        //{
        //    get => color;
        //    set
        //    {
        //        color = value;

        //        DirtyClasses();
        //    }
        //}

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
        /// The minimum value to accept for this input.
        /// </summary>
        [Parameter] public TValue Min { get; set; }

        /// <summary>
        /// The maximum value to accept for this input.
        /// </summary>
        [Parameter] public TValue Max { get; set; }

        #endregion
    }
}
