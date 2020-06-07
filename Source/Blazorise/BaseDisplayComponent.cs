#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Base component with support for display property.
    /// </summary>
    public abstract class BaseDisplayComponent : BaseComponent
    {
        #region Members

        private IFluentDisplay display;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            if ( Display != null )
                builder.Append( Display.Class( ClassProvider ) );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Specifies the display behavior of an element.
        /// </summary>
        [Parameter]
        public IFluentDisplay Display
        {
            get => display;
            set
            {
                display = value;

                DirtyClasses();
            }
        }

        #endregion
    }
}
