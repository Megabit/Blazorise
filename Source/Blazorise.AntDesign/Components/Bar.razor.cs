#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.AntDesign.Components;

public partial class Bar : Blazorise.Bar
{
    #region Members

    private BarMode initalMode = BarMode.Horizontal;

    #endregion

    #region Methods

    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.BarCollapsed( initalMode ), !Visible );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    [Parameter]
    public override bool Visible
    {
        get => base.Visible;
        set
        {
            // prevent bar from calling the same code multiple times
            if ( value == State.Visible )
                return;

            base.Mode = !value && CollapseMode == BarCollapseMode.Small ?
                BarMode.VerticalSmall : initalMode;

            base.Visible = value;
        }
    }

    [Parameter]
    public override BarMode Mode
    {
        get => base.Mode;
        set
        {
            if ( value == initalMode )
                return;

            initalMode = value;

            base.Mode = value;
        }
    }

    #endregion
}