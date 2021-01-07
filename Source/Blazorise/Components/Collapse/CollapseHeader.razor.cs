#region Using directives
using System.Threading.Tasks;
using Blazorise.Utilities;
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

        protected Task ClickHandler()
        {
            ParentCollapse.Toggle();

            return Task.CompletedTask;
        }

        #endregion

        #region Properties

        [CascadingParameter] protected Collapse ParentCollapse { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
