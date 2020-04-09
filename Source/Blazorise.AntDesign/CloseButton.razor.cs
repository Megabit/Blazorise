#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Blazorise.AntDesign
{
    public partial class CloseButton : Blazorise.CloseButton
    {
        #region Members

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            if ( ParentAlert != null )
                builder.Append( "ant-alert-close-icon" );

            if ( ParentModal != null )
                builder.Append( "ant-modal-close" );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        #endregion
    }
}
