#region Using directives
#endregion

namespace Blazorise.Bulma;

public partial class FieldLabel : Blazorise.FieldLabel
{
    #region Members

    #endregion

    #region Methods

    #endregion

    #region Properties

    protected override bool PreventColumnSize => true; // Bulma does not support column sizes on fields.

    #endregion
}