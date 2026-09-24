#region Using directives
using Blazorise.Utilities;
#endregion

namespace Blazorise.Bootstrap.Components;

public partial class Switch<TValue>
{
    private StyleBuilder labelStyleBuilder;

    protected override void OnInitialized()
    {
        labelStyleBuilder = new( BuildLabelStyles, builder => builder.Append( Style ) );

        base.OnInitialized();
    }

    protected internal override void DirtyStyles()
    {
        labelStyleBuilder?.Dirty();

        base.DirtyStyles();
    }

    private void BuildLabelStyles( StyleBuilder builder )
    {
        builder.Append( StyleProvider.SwitchColor( Color ) );
    }

    private string LabelStyleNames => labelStyleBuilder.Styles;
}