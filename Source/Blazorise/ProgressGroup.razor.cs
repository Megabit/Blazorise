#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public abstract class BaseProgressGroup : BaseComponent
    {
        #region Members

        private Size size = Size.None;

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.Progress() )
                .If( () => ClassProvider.ProgressSize( Size ), () => Size != Size.None );

            base.RegisterClasses();
        }

        #endregion

        #region Properties

        [Parameter]
        public Size Size
        {
            get => size;
            set
            {
                size = value;

                ClassMapper.Dirty();
            }
        }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
