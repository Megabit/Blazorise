#region Using directives

#endregion

namespace Blazorise.Tailwind;

public partial class Dropdown : Blazorise.Dropdown
{
    #region Members

    #endregion

    #region Methods

    //inheritdoc
    protected override string GetShowElementId()
        => ElementId;

    #endregion

    #region Properties


    #endregion
}