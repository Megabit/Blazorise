#region Using directives
using Blazorise.Utilities;
#endregion

namespace Blazorise.Frolic
{
    public partial class ProgressBar : Blazorise.ProgressBar
    {
        #region Members

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.ProgressBarSize( ParentProgress.Size ), ParentProgress != null && ParentProgress.Size != Size.None );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        #endregion
    }
}
