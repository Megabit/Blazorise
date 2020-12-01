#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Blazorise.Utils;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Checkboxes allow the user to select one or more items from a set.
    /// </summary>
    /// <typeparam name="TValue">Checked value type.</typeparam>
    public partial class Check<TValue> : BaseCheckComponent<TValue>
    {
        #region Members

        private bool? indeterminate;

        #endregion

        #region Methods

        public override async Task SetParametersAsync( ParameterView parameters )
        {
            await base.SetParametersAsync( parameters );

            if ( parameters.TryGetValue<bool?>( nameof( Indeterminate ), out var indeterminate ) && this.indeterminate != indeterminate )
            {
                this.indeterminate = indeterminate;

                ExecuteAfterRender( async () =>
                {
                    await JSRunner.SetProperty( ElementRef, "indeterminate", indeterminate );
                } );
            }
        }

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Check() );
            builder.Append( ClassProvider.CheckSize( Size ), Size != Size.None );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the string representation of a boolean "true" value.
        /// </summary>
        protected override string TrueValueName => "true";

        /// <summary>
        /// The indeterminate property can help you to achieve a 'check all' effect.
        /// </summary>
        [Parameter]
        public bool? Indeterminate { get; set; }

        #endregion
    }
}
