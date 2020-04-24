#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class CollapseHeader : BaseComponent
    {
        #region Members

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.CollapseHeader() );

            base.BuildClasses( builder );
        }

        protected void ClickHandler()
        {
            ParentCollapse.Toggle();
        }

        #endregion

        #region Properties

        [CascadingParameter] protected Collapse ParentCollapse { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
