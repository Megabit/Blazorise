#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public abstract class BasePaginationItem : BaseComponent
    {
        #region Members

        private bool isActive;

        private bool isDisabled;

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.PaginationItem() )
                .If( () => ClassProvider.PaginationItemActive(), () => IsActive )
                .If( () => ClassProvider.Disabled(), () => IsDisabled );

            base.RegisterClasses();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Indicate the currently active page.
        /// </summary>
        [Parameter]
        internal protected bool IsActive
        {
            get => isActive;
            set
            {
                isActive = value;

                ClassMapper.Dirty();
            }
        }

        /// <summary>
        /// Used for links that appear un-clickable.
        /// </summary>
        [Parameter]
        protected bool IsDisabled
        {
            get => isDisabled;
            set
            {
                isDisabled = value;

                ClassMapper.Dirty();
            }
        }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
