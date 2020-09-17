#region Using directives
using Microsoft.AspNetCore.Components;
#endregion


namespace Blazorise
{
    public partial class BarIcon : BaseComponent
    {
        #region Members

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( "b-bar-icon" );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        [Parameter] public object IconName { get; set; }

        #endregion
    }
}
