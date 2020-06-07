#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class JumbotronTitle : BaseComponent
    {
        #region Members

        private JumbotronTitleSize size = JumbotronTitleSize.Is1;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.JumbotronTitle( Size ) );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        [Parameter]
        public JumbotronTitleSize Size
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
