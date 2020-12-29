#region Using directives
using System;
using System.Globalization;
using System.Threading.Tasks;
using Blazorise.Utilities;
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
