#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class Media : BaseComponent
    {
        #region Members

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Media() );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
