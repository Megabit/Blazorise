#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class AlertDescription : BaseComponent
    {
        #region Members

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.AlertDescription() );

            base.BuildClasses( builder );
        }

        protected override void OnInitialized()
        {
            ParentAlert?.NotifyHasDescription();

            base.OnInitialized();
        }

        #endregion

        #region Properties

        [CascadingParameter] protected Alert ParentAlert { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
