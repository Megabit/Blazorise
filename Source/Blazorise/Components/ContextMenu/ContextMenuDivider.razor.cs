#region Using directives
using Blazorise.Utilities;
#endregion

namespace Blazorise;

/// <summary>
/// Divider that can be placed between <see cref="ContextMenuItem"/>'s.
/// </summary>
public partial class ContextMenuDivider : BaseComponent
{
    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.ContextMenuDivider() );

        base.BuildClasses( builder );
    }

    #endregion
}