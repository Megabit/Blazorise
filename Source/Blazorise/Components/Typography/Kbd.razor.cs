#region Using directives
using Blazorise.Utilities;
#endregion

namespace Blazorise;

/// <summary>
/// Represents user input, such as a key or keyboard shortcut, using the native <c>kbd</c> element.
/// </summary>
public partial class Kbd : BaseTypographyComponent
{
    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.Kbd() );

        base.BuildClasses( builder );
    }

    #endregion
}