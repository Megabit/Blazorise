#region Using directives
using Blazorise.Utilities;
#endregion

namespace Blazorise.Tailwind.Components;

/// <summary>
/// A color picker styled for Tailwind CSS.
/// </summary>
public partial class ColorPicker : Blazorise.ColorPicker
{
    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( "inline-flex items-center gap-2 text-left cursor-pointer disabled:cursor-not-allowed" );

        base.BuildClasses( builder );
    }

    #endregion
}