#region Using directives
using System.Collections.Concurrent;
#endregion

namespace Blazorise.Providers;

/// <summary>
/// Default implementation of <see cref="IIconProvider"/>.
/// </summary>
public abstract class BaseIconProvider : IIconProvider
{
    #region Members

    private readonly ConcurrentDictionary<string, string> customIcons = new();

    #endregion

    #region Methods

    /// <inheritdoc/>
    public virtual string Icon( object name, IconStyle style )
    {
        var iconStyle = GetStyleName( style );

        // Some icons must be placed inside of an icon tag element so just return 
        // the style name. The actual icon name will be defined in the Icon.razor file.
        if ( IconNameAsContent || name is null )
            return iconStyle;

        if ( name is IconName iconEnum )
        {
            return $"{iconStyle} {GetIconName( iconEnum, style )}".Trim();
        }
        else if ( name is string iconName )
        {
            if ( ContainsStyleName( iconName ) )
                return iconName;

            return $"{iconStyle} {iconName}".Trim();
        }

        return iconStyle;
    }

    /// <inheritdoc/>
    public abstract string IconSize( IconSize iconSize );

    /// <inheritdoc/>
    public abstract string GetIconName( IconName name, IconStyle style );

    /// <inheritdoc/>
    public abstract void SetIconName( IconName name, string newName );

    /// <inheritdoc/>
    public string GetIconName( string customName )
    {
        return customIcons.GetOrAdd( customName, customName );
    }

    /// <inheritdoc/>
    public abstract string GetStyleName( IconStyle iconStyle );

    /// <summary>
    /// Determines if style contains the icon name.
    /// </summary>
    /// <param name="iconName">Icon name to search.</param>
    /// <returns>True if icon name is found within the style.</returns>
    protected abstract bool ContainsStyleName( string iconName );

    #endregion

    #region Properties

    /// <summary>
    /// True if icon name should be placed as en element content.
    /// </summary>
    public abstract bool IconNameAsContent { get; }

    #endregion
}