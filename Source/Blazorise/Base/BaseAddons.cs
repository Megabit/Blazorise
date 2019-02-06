#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Base
{
    public abstract class BaseAddons : BaseComponent
    {
        #region Members

        private IFluentColumn columnSize;

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.Addons() );

            base.RegisterClasses();
        }

        #endregion

        #region Properties

        [Parameter]
        protected IFluentColumn ColumnSize
        {
            get => columnSize;
            set
            {
                columnSize = value;

                Dirty();
                ClassMapper.Dirty();
            }
        }

        protected virtual bool ParentIsHorizontal => ParentField?.IsHorizontal == true;

        [CascadingParameter] protected BaseField ParentField { get; set; }

        //protected bool IsInFieldBody => ParentFieldBody != null;

        [Parameter] protected RenderFragment ChildContent { get; set; }

        //[CascadingParameter] protected BaseFieldBody ParentFieldBody { get; set; }

        #endregion
    }
}
