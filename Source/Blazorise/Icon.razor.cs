#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class Icon : BaseComponent
    {
        #region Members

        private object name;

        private IconStyle iconStyle = IconStyle.Solid;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( IconProvider.Icon( Name, IconStyle ) );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        [Inject]
        protected IIconProvider IconProvider { get; set; }

        /// <summary>
        /// Icon name.
        /// </summary>
        [Parameter]
        public object Name
        {
            get => name;
            set
            {
                name = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Suggested icon style.
        /// </summary>
        [Parameter]
        public IconStyle IconStyle
        {
            get => iconStyle;
            set
            {
                iconStyle = value;

                DirtyClasses();
            }
        }

        #endregion
    }
}
