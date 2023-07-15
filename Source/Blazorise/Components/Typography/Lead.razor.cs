#region Using directives
using Blazorise.Utilities;
#endregion

namespace Blazorise;

/// <summary>
/// A typography component to make a paragraph stand out.
/// </summary>
public partial class Lead : BaseTypographyComponent
{
    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.Lead() );

        base.BuildClasses( builder );
    }

    #endregion
}