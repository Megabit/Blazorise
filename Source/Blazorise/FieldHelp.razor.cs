#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Sets the field help-text postioned bellow the field.
    /// </summary>
    public abstract class BaseFieldHelp : BaseComponent
    {
        #region Members

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.FieldHelp() );

            base.RegisterClasses();
        }

        #endregion

        #region Properties

        protected virtual bool ParentIsFieldBody => ParentFieldBody != null;

        [Parameter] public RenderFragment ChildContent { get; set; }

        [CascadingParameter] public BaseFieldBody ParentFieldBody { get; set; }

        #endregion
    }
}
