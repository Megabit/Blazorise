#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise
{
    public abstract class BaseDateEdit : BaseTextInput<DateTime?>
    {
        #region Members

        #endregion

        #region Methods

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

        protected void OnClickHandler( MouseEventArgs e )
        {
            JSRunner.ActivateDatePicker( ElementId, Utils.Parsers.InternalDateFormat );
        }

        protected override void OnInternalValueChanged( DateTime? value )
        {
            DateChanged.InvokeAsync( value );
        }

        protected override string FormatValueAsString( DateTime? value )
            => value?.ToString( Utils.Parsers.InternalDateFormat );

        protected override Task<ParseValue<DateTime?>> ParseValueFromStringAsync( string value )
        {
            if ( Utils.Parsers.TryParseDate( value, out var result ) )
            {
                return Task.FromResult( new ParseValue<DateTime?>( true, result, null ) );
            }
            else
            {
                return Task.FromResult( new ParseValue<DateTime?>( false, default, null ) );
            }
        }

        #endregion

        #region Properties

        protected override DateTime? InternalValue { get => Date; set => Date = value; }

        /// <summary>
        /// Gets or sets the input date value.
        /// </summary>
        [Parameter]
        public DateTime? Date { get; set; }

        /// <summary>
        /// Occurs when the date has changed.
        /// </summary>
        [Parameter] public EventCallback<DateTime?> DateChanged { get; set; }

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
