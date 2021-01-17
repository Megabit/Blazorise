#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class Jumbotron : BaseComponent
    {
        #region Members

        private Background background = Background.None;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Jumbotron() );
            builder.Append( ClassProvider.JumbotronBackground( Background ), Background != Background.None );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        [Parameter]
        public Background Background
        {
            get => background;
            set
            {
                background = value;

                DirtyClasses();
            }
        }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
