#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
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
        public bool IsFluid
        {
            get => isFluid;
            set
            {
                isFluid = value;

                ClassMapper.Dirty();
            }
        }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
