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
    public abstract class BaseDateEdit : BaseInputComponent<DateTime?>
    {
        #region Members

        protected string internalDate;

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.DateEdit() )
                .If( () => ClassProvider.DateEditSize( Size ), () => Size != Size.None )
                .If( () => ClassProvider.DateEditValidation( ParentValidation?.Status ?? ValidationStatus.None ), () => ParentValidation?.Status != ValidationStatus.None );

            base.RegisterClasses();
        }

        protected override void OnInitialized()
        {
            if ( ParentValidation != null )
            {
                ParentValidation.InitInputPattern( Pattern );
            }

            base.OnInitialized();
        }

        protected void ClickHandler( MouseEventArgs e )
        {
            JSRunner.ActivateDatePicker( ElementId, Utils.Parsers.InternalDateFormat );
        }

        protected Task InternalDateHandler( ChangeEventArgs e )
        {
            Date = Utils.Parsers.TryParseDate( e?.Value?.ToString() );
            return DateChanged.InvokeAsync( Date );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Sets the placeholder for the empty date.
        /// </summary>
        [Parameter] public string Placeholder { get; set; }

        /// <summary>
        /// Gets or sets the input date value.
        /// </summary>
        [Parameter]
        public DateTime? Date
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
        [Parameter] public EventCallback<DateTime?> DateChanged { get; set; }

        /// <summary>
        /// The earliest date to accept.
        /// </summary>
        [Parameter] public DateTime? Min { get; set; }

        /// <summary>
        /// The latest date to accept.
        /// </summary>
        [Parameter] public DateTime? Max { get; set; }

        /// <summary>
        /// The pattern attribute specifies a regular expression that the input element's value is checked against on form submission.
        /// </summary>
        [Parameter] public string Pattern { get; set; }

        #endregion
    }
}
