using Blazorise.Providers;

namespace Blazorise.Tests.bUnit;

class EmptyIconProvider : BaseIconProvider
{
    #region Members

    private static Dictionary<IconName, string> names = new()
    {
    };

    #endregion

    #region Methods

    public override string IconSize( IconSize iconSize )
    {
        return iconSize switch
        {
            _ => null,
        };
    }

    public override string GetIconName( IconName iconName, IconStyle iconStyle )
    {
        names.TryGetValue( iconName, out var name );

        return name;
    }

    public override void SetIconName( IconName name, string newName )
    {
        names[name] = newName;
    }

    public override string GetStyleName( IconStyle iconStyle ) => null;

    protected override bool ContainsStyleName( string iconName ) => false;

    #endregion

    #region Properties

    public override bool IconNameAsContent => false;

    #endregion
}
