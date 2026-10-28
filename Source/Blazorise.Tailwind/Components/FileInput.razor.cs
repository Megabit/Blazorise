#region Using directives
using Blazorise.Utilities;
#endregion

namespace Blazorise.Tailwind.Components;

public partial class FileInput
{
    #region Methods

    /// <inheritdoc />
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.FileInputSize( ThemeSize ) );

        base.BuildClasses( builder );
    }

    #endregion
}