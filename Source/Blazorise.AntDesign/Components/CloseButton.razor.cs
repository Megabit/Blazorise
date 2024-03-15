#region Using directives
using Blazorise.Utilities;
#endregion

namespace Blazorise.AntDesign.Components;

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

        if ( ParentToast != null )
            builder.Append( "ant-notification-notice-close" );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    #endregion
}