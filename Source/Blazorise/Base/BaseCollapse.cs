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
    public abstract class BaseCollapse : BaseComponent
    {
        #region Members

        private bool isOpen;

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .If( () => ClassProvider.Collapse(), () => IsOpen )
                .If( () => ClassProvider.CollapseShow(), () => IsOpen );

            base.RegisterClasses();
        }

        #endregion

        #region Properties

        [Parameter]
        protected bool IsOpen
        {
            get => isOpen;
            set
            {
                isOpen = value;

                ClassMapper.Dirty();
            }
        }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
