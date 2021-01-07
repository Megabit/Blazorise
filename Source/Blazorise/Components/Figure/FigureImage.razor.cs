#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class FigureImage : BaseComponent
    {
        #region Members

        private bool rounded;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.FigureImage() );
            builder.Append( ClassProvider.FigureImageRounded(), Rounded );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        [Parameter] public string Source { get; set; }

        [Parameter] public string AlternateText { get; set; }

        [Parameter]
        public bool Rounded
        {
            get => rounded;
            set
            {
                rounded = value;

                DirtyClasses();
            }
        }

        #endregion
    }
}
