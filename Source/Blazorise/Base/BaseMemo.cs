#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Base
{
    public abstract class BaseMemo : BaseInputComponent
    {
        #region Members

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.Memo() );

            base.RegisterClasses();
        }

        protected void HandleTextChange( UIChangeEventArgs e )
        {
            Text = e?.Value?.ToString();
            TextChanged?.Invoke( Text );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Sets the placeholder for the empty text.
        /// </summary>
        [Parameter] protected string Placeholder { get; set; }

        /// <summary>
        /// Gets or sets the text inside the input field.
        /// </summary>
        [Parameter] protected string Text { get; set; }

        /// <summary>
        /// Occurs after text has changed.
        /// </summary>
        [Parameter] protected Action<string> TextChanged { get; set; }

        /// <summary>
        /// Specifies the maximum number of characters allowed in the input element.
        /// </summary>
        [Parameter] protected int? MaxLength { get; set; }

        /// <summary>
        /// Specifies the number lines in the input element.
        /// </summary>
        [Parameter] protected int? Rows { get; set; }

        #endregion
    }
}
