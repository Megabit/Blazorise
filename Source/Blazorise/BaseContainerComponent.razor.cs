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
    /// Base class for components that are containers for other components.
    /// </summary>
    public abstract class BaseContainerComponent : BaseDisplayComponent
    {
        #region Members

        private IFluentColumn columnSize;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            if ( ColumnSize != null )
                builder.Append( ColumnSize.Class( ClassProvider ) );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Defines the column sizes.
        /// </summary>
        [Parameter]
        public IFluentColumn ColumnSize
        {
            get => columnSize;
            set
            {
                columnSize = value;

                DirtyClasses();
            }
        }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
