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
    public abstract class BaseButtons : BaseComponent
    {
        #region Members

        private ButtonsRole role = ButtonsRole.Addons;

        private Orientation orientation = Orientation.Horizontal;

        private Size size = Size.None;

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .If( () => ClassProvider.ButtonsAddons(), () => Role == ButtonsRole.Addons )
                .If( () => ClassProvider.ButtonsToolbar(), () => Role == ButtonsRole.Toolbar )
                .If( () => ClassProvider.ButtonsVertical(), () => Orientation == Orientation.Vertical )
                .If( () => ClassProvider.ButtonsSize( Size ), () => Size != Size.None );

            base.RegisterClasses();
        }

        #endregion

        #region Properties

        [Parameter]
        internal ButtonsRole Role
        {
            get => role;
            set
            {
                role = value;

                ClassMapper.Dirty();
            }
        }

        [Parameter]
        protected Orientation Orientation
        {
            get => orientation;
            set
            {
                orientation = value;

                ClassMapper.Dirty();
            }
        }

        [Parameter]
        protected Size Size
        {
            get => size;
            set
            {
                size = value;

                ClassMapper.Dirty();
            }
        }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
