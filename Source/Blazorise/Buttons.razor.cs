#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class Buttons : BaseComponent
    {
        #region Members

        private ButtonsRole role = ButtonsRole.Addons;

        private Orientation orientation = Orientation.Horizontal;

        private Size size = Size.None;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.ButtonsAddons(), Role == ButtonsRole.Addons );
            builder.Append( ClassProvider.ButtonsToolbar(), Role == ButtonsRole.Toolbar );
            builder.Append( ClassProvider.ButtonsVertical(), Orientation == Orientation.Vertical );
            builder.Append( ClassProvider.ButtonsSize( Size ), Size != Size.None );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        [Parameter]
        public ButtonsRole Role
        {
            get => role;
            set
            {
                role = value;

                DirtyClasses();
            }
        }

        [Parameter]
        public Orientation Orientation
        {
            get => orientation;
            set
            {
                orientation = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Change the size of multiple buttons at once.
        /// </summary>
        [Parameter]
        public Size Size
        {
            get => size;
            set
            {
                size = value;

                DirtyClasses();
            }
        }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
