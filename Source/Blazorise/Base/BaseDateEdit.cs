#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Base
{
    public abstract class BaseDateEdit : BaseInputComponent
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
            // link to the parent component
            ParentValidation?.Hook( this );

            base.OnInit();
        }

        protected internal override void Dirty()
        {
            ClassMapper.Dirty();

            base.Dirty();
        }

        protected void ClickHandler( UIMouseEventArgs e )
        {
            JSRunner.ActivateDatePicker( ElementId, Utils.Parsers.InternalDateFormat );
        }

        protected void InternalDateHandler( UIChangeEventArgs e )
        {
            Date = Utils.Parsers.TryParseDate( e?.Value?.ToString() );
            DateChanged?.Invoke( Date );

            ParentValidation?.InputValueChanged( Date );
        }

        #endregion

        #region Properties

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
                internalDate = value?.ToString( Utils.Parsers.InternalDateFormat );
            }
        }

        /// <summary>
        /// Occurs when the date has changed.
        /// </summary>
        [Parameter] private Action<DateTime?> DateChanged { get; set; }

        /// <summary>
        /// The earliest date to accept.
        /// </summary>
        [Parameter] protected DateTime? Min { get; set; }

        /// <summary>
        /// The latest date to accept.
        /// </summary>
        [Parameter] protected DateTime? Max { get; set; }

        [CascadingParameter] protected BaseValidation ParentValidation { get; set; }

        #endregion
    }
}
