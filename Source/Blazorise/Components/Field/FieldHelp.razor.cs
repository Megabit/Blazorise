#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Sets the field help-text positioned bellow the field.
    /// </summary>
    public partial class FieldHelp : BaseComponent
    {
        #region Members

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.FieldHelp() );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        protected virtual bool ParentIsFieldBody => ParentFieldBody != null;

        [Parameter] public RenderFragment ChildContent { get; set; }

        [CascadingParameter] protected FieldBody ParentFieldBody { get; set; }

        #endregion
    }
}
