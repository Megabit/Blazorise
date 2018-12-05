#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Blazor;
using Microsoft.AspNetCore.Blazor.Components;
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
                .If( () => ClassProvider.DateSize( Size ), () => Size != Size.None );

            base.RegisterClasses();
        }

        protected void ClickHandler( UIMouseEventArgs e )
        {
            JSRunner.ActivateDatePicker( ElementId );
        }

        protected void InternalDateHandler( UIChangeEventArgs e )
        {
            Date = Utils.Parsers.TryParseDate( e?.Value?.ToString() );
            DateChanged?.Invoke( Date );
        }

        #endregion

        #region Properties

        [Parameter] protected string Placeholder { get; set; }

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

        #endregion
    }
}
