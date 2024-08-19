#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.AntDesign.Components;

public partial class Bar : Blazorise.Bar
{
    #region Members

    private BarMode initialMode = BarMode.Horizontal;

    #endregion

    #region Methods

    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.BarCollapsed( initialMode, Visible ) );

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
                BarMode.VerticalSmall : initialMode;

            base.Visible = value;
        }
    }

    [Parameter]
    public override BarMode Mode
    {
        get => base.Mode;
        set
        {
            if ( value == initialMode )
                return;

            initialMode = value;

            base.Mode = value;
        }
    }

    #endregion
}