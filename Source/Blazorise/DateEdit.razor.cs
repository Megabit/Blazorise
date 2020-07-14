#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Blazorise.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise
{
    public partial class DateEdit<TValue> : BaseTextInput<TValue>
    {
        #region Members

        #endregion

        #region Methods

        protected override void OnInitialized()
        {
            if ( ParentValidation != null )
            {
                ParentValidation.InitializeInputExpression( DateExpression );
            }

            base.OnInitialized();
        }

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.DateEdit() );
            builder.Append( ClassProvider.DateEditSize( Size ), Size != Size.None );
            builder.Append( ClassProvider.DateEditValidation( ParentValidation?.Status ?? ValidationStatus.None ), ParentValidation?.Status != ValidationStatus.None );

            base.BuildClasses( builder );
        }

        protected override Task OnChangeHandler( ChangeEventArgs e )
        {
            return CurrentValueHandler( e?.Value?.ToString() );
        }

        protected async Task OnClickHandler( MouseEventArgs e )
        {
            await JSRunner.ActivateDatePicker( ElementId, Parsers.InternalDateFormat );
        }

        protected override Task OnInternalValueChanged( TValue value )
        {
            return DateChanged.InvokeAsync( value );
        }

        protected override string FormatValueAsString( TValue value )
        {
            switch ( value )
            {
                case null:
                    return null;
                case DateTime datetime:
                    return datetime.ToString( Parsers.InternalDateFormat );
                case DateTimeOffset datetimeOffset:
                    return datetimeOffset.ToString( Parsers.InternalDateFormat );
                default:
                    throw new InvalidOperationException( $"Unsupported type {value.GetType()}" );
            }
        }

        protected override Task<ParseValue<TValue>> ParseValueFromStringAsync( string value )
        {
            if ( Parsers.TryParseDate<TValue>( value, out var result ) )
            {
                return Task.FromResult( new ParseValue<TValue>( true, result, null ) );
            }
            else
            {
                return Task.FromResult( new ParseValue<TValue>( false, default, null ) );
            }
        }

        #endregion

        #region Properties

        protected override TValue InternalValue { get => Date; set => Date = value; }

        /// <summary>
        /// Gets or sets the input date value.
        /// </summary>
        [Parameter]
        public TValue Date { get; set; }

        /// <summary>
        /// Occurs when the date has changed.
        /// </summary>
        [Parameter] public EventCallback<TValue> DateChanged { get; set; }

        /// <summary>
        /// Gets or sets an expression that identifies the date value.
        /// </summary>
        [Parameter] public Expression<Func<TValue>> DateExpression { get; set; }

        /// <summary>
        /// The earliest date to accept.
        /// </summary>
        [Parameter] public DateTime? Min { get; set; }

        /// <summary>
        /// The latest date to accept.
        /// </summary>
        [Parameter] public DateTime? Max { get; set; }

        #endregion
    }
}
