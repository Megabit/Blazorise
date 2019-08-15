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
    public abstract class BaseContainerComponent : BaseComponent
    {
        #region Members

        private IFluentColumn columnSize;

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .If( () => ColumnSize.Class( ClassProvider ), () => ColumnSize != null );

            base.RegisterClasses();
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

                ClassMapper.Dirty();
            }
        }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
