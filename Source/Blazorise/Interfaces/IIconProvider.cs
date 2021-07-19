#region Using directives
#endregion

namespace Blazorise
{
    /// <summary>
    /// Default contract for all icon providers.
    /// </summary>
    public interface IIconProvider
    {
        /// <summary>
        /// Gets the predefined icon style.
        /// </summary>
        /// <param name="name">Icon name.</param>
        /// <param name="iconStyle">Icon style.</param>
        /// <returns>Icon name.</returns>
        string Icon( object name, IconStyle iconStyle );

        /// <summary>
        /// Gets the predefined icon size classname.
        /// </summary>
        /// <param name="iconSize">Icon size.</param>
        /// <returns>The size classname.</returns>
        string IconSize( IconSize iconSize );

        /// <summary>
        /// Gets the icon name by predefined icon type.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="iconStyle">Icon style.</param>
        /// <returns>Icon name.</returns>
        string GetIconName( IconName name, IconStyle iconStyle );

        /// <summary>
        /// Overrides the predefined icon name.
        /// </summary>
        /// <param name="name">Icon to override.</param>
        /// <param name="newName">New icon name.</param>
        void SetIconName( IconName name, string newName );

        /// <summary>
        /// Gets the icon name by the custom icon name.
        /// </summary>
        /// <param name="customName"></param>
        /// <returns>Icon name.</returns>
        string GetIconName( string customName );

        /// <summary>
        /// Indicates if the classname will be defined as tag content. 
        /// </summary>
        bool IconNameAsContent { get; }
    }
}
