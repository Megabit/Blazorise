#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// A wrapper component around <see cref="Icon"/> that is used by the <see cref="Bar"/> component.
/// </summary>
public partial class BarIcon : BaseComponent
{
    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        BuildBarIconClasses( builder );

        base.BuildClasses( builder );
    }

    /// <summary>
    /// Builds class names for bar icon.
    /// </summary>
    /// <param name="builder">Class builder.</param>
    protected virtual void BuildBarIconClasses( ClassBuilder builder )
    {
        builder.Append( "b-bar-icon" );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Icon name that can be either a string or <see cref="IconName"/>.
    /// </summary>
    [Parameter] public object IconName { get; set; }

    /// <summary>
    /// Suggested icon style.
    /// </summary>
    [Parameter] public IconStyle IconStyle { get; set; }

    /// <summary>
    /// Defines the icon size.
    /// </summary>
    [Parameter] public IconSize IconSize { get; set; }

    #endregion
}