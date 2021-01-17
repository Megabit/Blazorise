#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class Figure : BaseComponent
    {
        #region Members

        private FigureSize size = FigureSize.None;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Figure() );
            builder.Append( ClassProvider.FigureSize( Size ), Size != FigureSize.None );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        [Parameter]
        public FigureSize Size
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
