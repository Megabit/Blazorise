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
    public abstract class BaseContainer : BaseComponent
    {
        #region Members

        private bool isFluid;

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .If( () => ClassProvider.ContainerFluid(), () => IsFluid )
                .If( () => ClassProvider.Container(), () => !IsFluid );

            base.RegisterClasses();
        }

        #endregion

        #region Properties

        [Parameter]
        protected bool IsFluid
        {
            get => isFluid;
            set
            {
                isFluid = value;

                ClassMapper.Dirty();
            }
        }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
