#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Base
{
    public abstract class BaseDateEdit : BaseInputComponent<DateTime?>
    {
        #region Members

        protected string internalDate;

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.Date() )
                .If( () => ClassProvider.DateSize( Size ), () => Size != Size.None )
                .If( () => ClassProvider.DateValidation( ParentValidation?.Status ?? ValidationStatus.None ), () => ParentValidation?.Status != ValidationStatus.None );

            base.RegisterClasses();
        }

        protected override void OnInit()
        {
            if ( ParentValidation != null )
            {
                ParentValidation.InitInputPattern( Pattern );
            }

            base.OnInit();
        }

        protected void ClickHandler( UIMouseEventArgs e )
        {
            JSRunner.ActivateDatePicker( ElementId, Utils.Parsers.InternalDateFormat );
        }

        protected Task InternalDateHandler( UIChangeEventArgs e )
        {
            Date = Utils.Parsers.TryParseDate( e?.Value?.ToString() );
            return DateChanged.InvokeAsync( Date );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Sets the placeholder for the empty date.
        /// </summary>
        [Parameter] protected string Placeholder { get; set; }

        /// <summary>
        /// Gets or sets the input date value.
        /// </summary>
        [Parameter]
        protected DateTime? Date
        {
            get
            {
                return string.IsNullOrEmpty( internalDate ) ? null : Utils.Parsers.TryParseDate( internalDate );
            }
            set
            {
                InternalValue = value;
                internalDate = InternalValue?.ToString( Utils.Parsers.InternalDateFormat );
            }
        }

        /// <summary>
        /// Occurs when the date has changed.
        /// </summary>
        [Parameter] private EventCallback<DateTime?> DateChanged { get; set; }

        /// <summary>
        /// The earliest date to accept.
        /// </summary>
        [Parameter] protected DateTime? Min { get; set; }

        /// <summary>
        /// The latest date to accept.
        /// </summary>
        [Parameter] protected DateTime? Max { get; set; }

        /// <summary>
        /// The pattern attribute specifies a regular expression that the input element's value is checked against on form submission.
        /// </summary>
        [Parameter] protected string Pattern { get; set; }

        #endregion
    }
}
