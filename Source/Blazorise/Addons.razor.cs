#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class Addons : BaseComponent
    {
        #region Members

        private IFluentColumn columnSize;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Addons() );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        [Parameter]
        public IFluentColumn ColumnSize
        {
            get => columnSize;
            set
            {
                columnSize = value;

                DirtyClasses();
                DirtyClasses();
            }
        }

        protected virtual bool ParentIsHorizontal => ParentField?.IsHorizontal == true;

        [CascadingParameter] protected BaseField ParentField { get; set; }

        //protected bool IsInFieldBody => ParentFieldBody != null;

        [Parameter] public RenderFragment ChildContent { get; set; }

        //[CascadingParameter] protected BaseFieldBody ParentFieldBody { get; set; }

        #endregion
    }
}
